using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
  [Header("Component References")]
  private PlayerStats playerStats;
  private IPlayerInput playerInput;
  private Rigidbody2D rb;

  [SerializeField] private Transform teleportTarget; // For teleportation functionality

  private StateMachine stateMachine;
  private Dictionary<PlayerState, IState> states;
  private IPlayerMovement playerMovement;

  private bool isGrounded = false;
  private bool isJumping = false;
  private bool isFacingRight = true;

  // Enhanced jump tracking
  private bool wasGroundedLastFrame = false;
  private bool jumpPressedThisFrame = false;
  private bool jumpReleasedThisFrame = false;
  private float lastJumpTime = 0f; // Track when last jump occurred
  private float minAirborneTime = 0.05f; // Reduced for immediate double jump responsiveness

  // Double jump input buffering for reliability
  private bool jumpInputBuffer = false;
  private float jumpInputBufferTimer = 0f;
  private float jumpInputBufferDuration = 0.25f; // Increased from 0.15f for more forgiving double jump timing

  // DEAD CELLS: Double Jump System
  [Header("Dead Cells Jump System")]
  [SerializeField] private int maxJumps = 2; // Double jump
  private int currentJumps = 0;
  private bool hasUsedDoubleJump = false;

  // Ground detection improvement
  private int groundContactCount = 0;

  // Dash cooldown system
  [Header("Dash Cooldown")]
  [SerializeField] private float dashCooldownTime = 0.5f; // 1 second cooldown
  private float dashCooldownTimer = 0f;
  private float lastDashTime = -10f; // Initialize to allow immediate first dash

  public PlayerStats PlayerStats => playerStats;
  public IPlayerInput PlayerInput => playerInput;
  public IPlayerMovement PlayerMovement => playerMovement;

  public bool IsFacingRight => isFacingRight;
  public bool IsGrounded => isGrounded;

  // Dash cooldown properties
  public float DashCooldownTime => dashCooldownTime;
  public float DashCooldownRemaining => dashCooldownTimer;
  public bool IsDashOnCooldown => dashCooldownTimer > 0f;
  public float DashCooldownPercentage => dashCooldownTimer / dashCooldownTime;

  private void Awake()
  {
    InitializeComponents();
    InitializeStateMachine();
  }

  private void InitializeComponents()
  {
    rb = GetComponent<Rigidbody2D>();
    playerStats = GetComponent<PlayerStats>();

    playerMovement = new NormalMovement();
    playerMovement.Initialize(playerStats);

    playerInput = new KeyboardInput();
  }

  private void InitializeStateMachine()
  {
    states = new Dictionary<PlayerState, IState>
        {
            { PlayerState.Locomotion, new LocomotionState(this) },
            { PlayerState.Charge, new ChargeState(this) },
            { PlayerState.Shield, new ShieldState(this) },
            { PlayerState.Dash, new DashState(this) },
            { PlayerState.Attack1, new Attack1State(this) },
            { PlayerState.Attack2, new Attack2State(this) },
            { PlayerState.Attack3, new Attack3State(this) },
            { PlayerState.Attack4, new Attack4State(this) },
            { PlayerState.Hurt, new HurtState(this) },
            { PlayerState.Die, new DieState(this) },
            { PlayerState.Air, new AirState(this) },
        };

    stateMachine = new StateMachine();
    stateMachine.Initialize(GetState(PlayerState.Locomotion));
  }

  private void Update()
  {

    if (Input.GetKeyDown(KeyCode.F))
    {
      Debug.Log("Teleporting to target position");
      if (teleportTarget != null)
      {
        transform.position = teleportTarget.position;
      }
      else
      {
        Debug.LogWarning("Teleport target not set!");
      }
    }

    // Update input tracking for enhanced jump features
    UpdateInputTracking();

    // Update movement system timers
    UpdateMovementTimers();

    // Update dash cooldown
    UpdateDashCooldown();

    // Debug state transitions (F1 key)
    DebugStateTransitions();

    stateMachine.Update();
  }

  private void FixedUpdate()
  {
    HandleFacingDirection();
    stateMachine.FixedUpdate();

    // Update frame tracking
    wasGroundedLastFrame = isGrounded;
  }

  private void UpdateInputTracking()
  {
    // Track jump input for buffering and variable height
    jumpPressedThisFrame = playerInput.IsJumpPressed();
    jumpReleasedThisFrame = !playerInput.IsJumpHeld() && isJumping && rb.linearVelocity.y > 0;

    // IMPROVED JUMP INPUT BUFFERING: Better handling for double jump
    if (jumpPressedThisFrame)
    {
      // Always refresh buffer on new input, even if already buffered
      jumpInputBuffer = true;
      jumpInputBufferTimer = jumpInputBufferDuration;
      Debug.Log($"Jump input buffered for {jumpInputBufferDuration}s (refreshed on new press)");
    }

    // Decrease buffer timer - but keep it active longer for double jumps
    if (jumpInputBufferTimer > 0f)
    {
      jumpInputBufferTimer -= Time.deltaTime;
      if (jumpInputBufferTimer <= 0f)
      {
        jumpInputBuffer = false;
        Debug.Log("Jump input buffer expired");
      }
    }

    // Handle jump cut for variable jump height
    if (jumpReleasedThisFrame && !playerMovement.IsJumpCut())
    {
      playerMovement.CutJump();
    }
  }

  private void UpdateMovementTimers()
  {
    // CRITICAL FIX: Ensure movement system knows the correct ground state
    playerMovement.UpdateCoyoteTime(isGrounded);

    // Update jump buffer
    playerMovement.UpdateJumpBuffer(jumpPressedThisFrame);

    // CRITICAL FIX: If player is grounded but movement thinks we're under external control, clear it
    if (isGrounded && playerMovement.IsUnderExternalControl())
    {
      // Check if external control should have expired
      Debug.Log("Ground detected - checking if external control should be cleared");
      // Let the movement system handle its own cleanup, but log for debugging
    }
  }

  private void UpdateDashCooldown()
  {
    if (dashCooldownTimer > 0f)
    {
      dashCooldownTimer -= Time.deltaTime;
      if (dashCooldownTimer < 0f)
      {
        dashCooldownTimer = 0f;
      }
    }
  }

  private void HandleFacingDirection()
  {
    float movementInput = playerInput.GetMovementInput();

    // Only change direction if there's significant input to avoid jittery turning
    // Dead Cells/Celeste style: more responsive direction changes
    if (Mathf.Abs(movementInput) > 0.2f)
    {
      if (movementInput > 0f && !isFacingRight)
      {
        transform.localScale = new Vector3(1, 1, 1);
        isFacingRight = true;
      }
      else if (movementInput < 0f && isFacingRight)
      {
        transform.localScale = new Vector3(-1, 1, 1);
        isFacingRight = false;
      }
    }
  }

  public void StopMovement()
  {
    rb.linearVelocity = Vector2.zero;
  }

  public bool IsWalking()
  {
    if (!isGrounded) return false;

    // Check both input AND actual movement velocity for accurate state detection
    bool hasInput = Mathf.Abs(playerInput.GetMovementInput()) > 0.1f;
    bool isActuallyMoving = playerMovement.IsMovingHorizontally();

    return hasInput || isActuallyMoving;
  }

  public bool IsIdling()
  {
    if (!isGrounded) return false;

    // Only idle if no input AND not actually moving AND no other actions
    bool noInput = Mathf.Abs(playerInput.GetMovementInput()) <= 0.1f;
    bool notMoving = !playerMovement.IsMovingHorizontally();
    bool noOtherActions = !playerInput.IsChargeHeld() && !playerInput.IsShieldHeld();

    return noInput && notMoving && noOtherActions && !playerMovement.IsUnderExternalControl();
  }

  public bool IsCharging() => playerInput.IsChargeHeld() && isGrounded;

  public bool IsShielding() => playerInput.IsShieldHeld() && isGrounded;

  public bool IsDashing()
  {
    // DEAD CELLS: Allow dash both on ground AND in air
    bool canDash = playerInput.IsDashPressed() && dashCooldownTimer <= 0f;

    // Don't allow dash if already under external control (already dashing)
    if (playerMovement.IsUnderExternalControl())
    {
      Debug.Log("IsDashing: false - under external control (already dashing)");
      return false;
    }

    if (playerInput.IsDashPressed())
    {
      Debug.Log($"=== DASH INPUT DETECTED ===");
      Debug.Log($"IsDashing: {canDash} - grounded: {isGrounded}, dashPressed: {playerInput.IsDashPressed()}, cooldown: {dashCooldownTimer}");
      Debug.Log($"IsWalking: {IsWalking()}, IsIdling: {IsIdling()}, IsCharging: {IsCharging()}, IsShielding: {IsShielding()}");
      Debug.Log($"Movement Input: {playerInput.GetMovementInput()}");
      Debug.Log($"Under External Control: {playerMovement.IsUnderExternalControl()}");
    }

    return canDash;
  }

  public bool IsJumping()
  {
    // Check if jump input was pressed this frame OR if we have buffered input
    bool hasJumpInput = playerInput.IsJumpPressed() || jumpInputBuffer;
    if (!hasJumpInput)
    {
      return false; // No new jump input, return false
    }

    Debug.Log($"=== JUMP INPUT DETECTED ===");
    Debug.Log($"Current Jumps: {currentJumps}, Has Used Double Jump: {hasUsedDoubleJump}");
    Debug.Log($"Is Grounded: {isGrounded}, Can Coyote: {playerMovement.CanCoyoteJump()}");

    // Ground jump or coyote time (first jump)
    if (isGrounded || playerMovement.CanCoyoteJump())
    {
      // Always allow ground jump - reset counters first
      currentJumps = 1; // First jump
      hasUsedDoubleJump = false;
      isJumping = true;
      lastJumpTime = Time.time; // Track jump time for ground detection
      ClearJumpBuffer(); // Clear buffer since jump was successful
      Debug.Log("FIRST JUMP ACTIVATED!");
      return true;
    }
    // DOUBLE JUMP - can be used anytime in air without waiting for fall state
    else if (currentJumps == 1 && !hasUsedDoubleJump)
    {
      // FIXED: Check if enough time has passed since first jump for valid double jump
      float timeSinceLastJump = Time.time - lastJumpTime;
      if (timeSinceLastJump >= 0.1f) // Minimum 0.1s delay for clean double jump
      {
        currentJumps = 2; // Second jump (maximum)
        hasUsedDoubleJump = true;
        isJumping = true;
        lastJumpTime = Time.time; // Track jump time for ground detection
        ClearJumpBuffer(); // Clear buffer since jump was successful
        Debug.Log("DOUBLE JUMP ACTIVATED!");
        return true;
      }
      else
      {
        // TOO EARLY: Don't consume the jump, keep the buffer for a valid attempt
        Debug.Log($"Double jump too early - need {0.1f - timeSinceLastJump:F2}s more. Keeping buffer.");
        return false; // Don't clear buffer, allow retry
      }
    }
    else
    {
      // INVALID: Clear buffer since no jumps are available
      if (currentJumps >= 2 || hasUsedDoubleJump)
      {
        ClearJumpBuffer();
        Debug.Log($"JUMP BLOCKED: No jumps remaining (currentJumps = {currentJumps}, hasUsedDoubleJump = {hasUsedDoubleJump})");
      }
      else
      {
        Debug.Log($"JUMP BLOCKED: Unknown condition (currentJumps = {currentJumps}, hasUsedDoubleJump = {hasUsedDoubleJump})");
      }
    }

    // No more jumps available
    return false;
  }

  // Helper method to clear jump input buffer
  private void ClearJumpBuffer()
  {
    jumpInputBuffer = false;
    jumpInputBufferTimer = 0f;
    Debug.Log("Jump input buffer cleared");
  }

  public bool CanAttack() => isGrounded && playerInput.IsAttackPressed();

  public bool IsHurt() => playerStats.IsHurt;

  public bool IsFalling() => rb.linearVelocity.y < -0.5f && !isGrounded;

  // Debug method to help identify dash issues
  public void DebugDashState()
  {
    Debug.Log($"=== DASH DEBUG ===");
    Debug.Log($"Dash Key Pressed: {playerInput.IsDashPressed()}");
    Debug.Log($"Is Grounded: {isGrounded}");
    Debug.Log($"Is Dashing: {IsDashing()}");
    Debug.Log($"Dash Cooldown: {dashCooldownTimer:F2}s remaining");
    Debug.Log($"Dash On Cooldown: {IsDashOnCooldown}");
    Debug.Log($"Under External Control: {playerMovement.IsUnderExternalControl()}");
    Debug.Log($"Movement Input: {playerInput.GetMovementInput()}");
    Debug.Log($"Is Walking: {IsWalking()}");
    Debug.Log($"Is Idling: {IsIdling()}");
    Debug.Log($"Is Moving Horizontally: {playerMovement.IsMovingHorizontally()}");
    Debug.Log($"Current State: {stateMachine.CurrentState?.GetType().Name}");
  }

  // Method to start dash cooldown (called when dash begins)
  public void StartDashCooldown()
  {
    dashCooldownTimer = dashCooldownTime;
    lastDashTime = Time.time;
    Debug.Log($"Dash cooldown started: {dashCooldownTime}s");
  }

  // Method to reset jumping state (called when landing)
  public void ResetJumpingState()
  {
    isJumping = false;
    currentJumps = 0;
    hasUsedDoubleJump = false;
    jumpInputBuffer = false; // Clear any lingering input buffer
    jumpInputBufferTimer = 0f;
    Debug.Log($"=== JUMP STATE RESET - PLAYER LANDED ===");
    Debug.Log($"Reset: isJumping={isJumping}, currentJumps={currentJumps}, hasUsedDoubleJump={hasUsedDoubleJump}");
  }

  // Method to clear just the jumping flag (called when falling)
  public void ClearJumpingFlag()
  {
    isJumping = false;
    Debug.Log($"=== JUMPING FLAG CLEARED - PLAYER FALLING ===");
    Debug.Log($"After Clear: isJumping={isJumping}, currentJumps={currentJumps}, hasUsedDoubleJump={hasUsedDoubleJump}");
  }

  // Method to clear jumping input state after processing
  public void ClearJumpingInput()
  {
    isJumping = false;
    jumpInputBuffer = false;
    Debug.Log("=== JUMPING INPUT CLEARED ===");
  }

  // Call this in Update for debugging
  private void DebugStateTransitions()
  {
    if (Input.GetKeyDown(KeyCode.F1))
    {
      DebugDashState();
    }

    if (Input.GetKeyDown(KeyCode.F2))
    {
      DebugCombatState();
    }

    if (Input.GetKeyDown(KeyCode.F3))
    {
      DebugJumpState();
    }

    if (Input.GetKeyDown(KeyCode.F4))
    {
      DebugCombatTiming();
    }

    // NEW: Emergency fix for stuck states
    if (Input.GetKeyDown(KeyCode.F5))
    {
      DebugForceUnstuck();
    }
  }

  // Emergency method to force unstuck player
  public void DebugForceUnstuck()
  {
    Debug.Log("=== EMERGENCY UNSTUCK ACTIVATED ===");
    Debug.Log($"Current State: {GetCurrentStateName()}");
    Debug.Log($"Is Grounded: {isGrounded}");
    Debug.Log($"Under External Control: {playerMovement.IsUnderExternalControl()}");
    Debug.Log($"Y Velocity: {rb.linearVelocity.y}");

    // Force clear external control
    playerMovement.ForceStopExternalControl();

    // Reset jump state
    ResetJumpingState();

    // Force transition to appropriate state
    if (isGrounded)
    {
      Debug.Log("Force transitioning to Locomotion");
      stateMachine.Initialize(GetState(PlayerState.Locomotion));
    }
    else
    {
      Debug.Log("Force transitioning to Air");
      stateMachine.Initialize(GetState(PlayerState.Air));
    }
  }

  // Debug method for grace period combat timing
  public void DebugCombatTiming()
  {
    Debug.Log($"=== GRACE PERIOD COMBAT TIMING DEBUG ===");
    Debug.Log($"Current State: {GetCurrentStateName()}");

    if (stateMachine.CurrentState is BasePlayerAttackState attackState)
    {
      // Use reflection to access timing details for comprehensive debugging
      var stateTimerField = typeof(BasePlayerAttackState).GetField("stateTimer",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var animationDurationField = typeof(BasePlayerAttackState).GetField("animationDuration",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var comboGracePeriodField = typeof(BasePlayerAttackState).GetField("comboGracePeriod",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var hasMinTimeField = typeof(BasePlayerAttackState).GetField("hasMinAnimationTimePassed",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var canComboField = typeof(BasePlayerAttackState).GetField("canCombo",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

      if (stateTimerField != null && animationDurationField != null && comboGracePeriodField != null)
      {
        float stateTimer = (float)stateTimerField.GetValue(attackState);
        float animationDuration = (float)animationDurationField.GetValue(attackState);
        float comboGracePeriod = (float)comboGracePeriodField.GetValue(attackState);
        bool hasMinTime = (bool)hasMinTimeField.GetValue(attackState);
        bool canCombo = (bool)canComboField.GetValue(attackState);

        float totalWindow = animationDuration + comboGracePeriod;
        bool inGracePeriod = stateTimer > animationDuration && stateTimer <= totalWindow;

        Debug.Log($"=== GRACE PERIOD TIMING ===");
        Debug.Log($"State Timer: {stateTimer:F3}s");
        Debug.Log($"Animation Duration: {animationDuration:F2}s");
        Debug.Log($"Grace Period: {comboGracePeriod:F2}s");
        Debug.Log($"Total Window: {totalWindow:F2}s");
        Debug.Log($"Animation Complete: {stateTimer >= animationDuration}");
        Debug.Log($"In Grace Period: {inGracePeriod}");
        Debug.Log($"Can Combo: {canCombo}");
        Debug.Log($"Min Time Passed: {hasMinTime}");

        if (inGracePeriod)
        {
          float gracePeriodRemaining = totalWindow - stateTimer;
          Debug.Log($"GRACE PERIOD ACTIVE - {gracePeriodRemaining:F3}s remaining");
        }
        else if (stateTimer > totalWindow)
        {
          Debug.Log($"COMBO WINDOW EXPIRED - Exceeded by {(stateTimer - totalWindow):F3}s");
        }
      }
    }
    else
    {
      Debug.Log("Not in attack state - grace period system inactive");
    }
  }

  // Debug method for jump system
  public void DebugJumpState()
  {
    Debug.Log($"=== JUMP SYSTEM DEBUG ===");
    Debug.Log($"Current Jumps: {currentJumps}");
    Debug.Log($"Max Jumps: {maxJumps}");
    Debug.Log($"Has Used Double Jump: {hasUsedDoubleJump}");
    Debug.Log($"Is Jumping Flag: {isJumping}");
    Debug.Log($"Is Grounded: {isGrounded}");
    Debug.Log($"Can Coyote Jump: {playerMovement.CanCoyoteJump()}");
    Debug.Log($"Jump Input Pressed: {playerInput.IsJumpPressed()}");
    Debug.Log($"Jump Input Held: {playerInput.IsJumpHeld()}");
    Debug.Log($"Jump Input Buffer: {jumpInputBuffer}");
    Debug.Log($"Jump Buffer Timer: {jumpInputBufferTimer:F3}s");
    Debug.Log($"Current State: {GetCurrentStateName()}");
    Debug.Log($"Is Falling: {IsFalling()}");
    Debug.Log($"Y Velocity: {rb.linearVelocity.y}");
  }

  // Debug method for combat system
  public void DebugCombatState()
  {
    Debug.Log($"=== PROFESSIONAL COMBAT DEBUG ===");
    Debug.Log($"Current State: {GetCurrentStateName()}");
    Debug.Log($"Can Attack: {CanAttack()}");
    Debug.Log($"Attack Pressed: {playerInput.IsAttackPressed()}");
    Debug.Log($"Attack Held: {playerInput.IsAttackHeld()}");
    Debug.Log($"Is Grounded: {isGrounded}");

    if (stateMachine.CurrentState is BasePlayerAttackState attackState)
    {
      Debug.Log($"=== ATTACK STATE DETAILS ===");
      Debug.Log($"Attack State Type: {attackState.GetType().Name}");

      // Use reflection to access timing details for comprehensive debugging
      var stateTimerField = typeof(BasePlayerAttackState).GetField("stateTimer",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var hasMinTimeField = typeof(BasePlayerAttackState).GetField("hasMinAnimationTimePassed",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var minDisplayTimeField = typeof(BasePlayerAttackState).GetField("minAnimationDisplayTime",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var canComboField = typeof(BasePlayerAttackState).GetField("canCombo",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var isInEarlyComboField = typeof(BasePlayerAttackState).GetField("isInEarlyComboWindow",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var hasInputBufferedField = typeof(BasePlayerAttackState).GetField("hasInputBuffered",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
      var comboBufferTimerField = typeof(BasePlayerAttackState).GetField("comboBufferTimer",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

      if (stateTimerField != null)
      {
        float stateTimer = (float)stateTimerField.GetValue(attackState);
        bool hasMinTime = (bool)hasMinTimeField.GetValue(attackState);
        float minDisplayTime = (float)minDisplayTimeField.GetValue(attackState);
        bool canCombo = (bool)canComboField.GetValue(attackState);
        bool isInEarlyCombo = (bool)isInEarlyComboField.GetValue(attackState);
        bool hasInputBuffered = (bool)hasInputBufferedField.GetValue(attackState);
        float comboBufferTimer = (float)comboBufferTimerField.GetValue(attackState);

        Debug.Log($"=== ANIMATION TIMING SYSTEM ===");
        Debug.Log($"State Timer: {stateTimer:F3}s");
        Debug.Log($"Min Display Time: {minDisplayTime:F2}s");
        Debug.Log($"Min Time Passed: {hasMinTime}");
        Debug.Log($"Can Combo: {canCombo}");
        Debug.Log($"In Early Combo Window: {isInEarlyCombo}");
        Debug.Log($"Has Input Buffered: {hasInputBuffered}");
        Debug.Log($"Combo Buffer Timer: {comboBufferTimer:F3}s");
        Debug.Log($"=== TRANSITION ANALYSIS ===");

        if (hasInputBuffered || playerInput.IsAttackPressed())
        {
          if (!hasMinTime)
          {
            Debug.Log($"COMBO BLOCKED: Waiting for minimum animation time");
            Debug.Log($"Time remaining: {(minDisplayTime - stateTimer):F3}s");
          }
          else if (!canCombo)
          {
            Debug.Log($"COMBO BLOCKED: Not in combo window yet");
          }
          else
          {
            Debug.Log($"COMBO READY: All conditions met for transition");
          }
        }
      }
    }
  }

  // Public method to get current state name for debugging
  public string GetCurrentStateName()
  {
    return stateMachine.CurrentState?.GetType().Name ?? "Unknown";
  }

  // Debug method to get current jump count
  public int GetCurrentJumps()
  {
    return currentJumps;
  }

  public IState GetState(PlayerState state) =>
      states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Rock"))
    {
      // More precise ground detection for better platformer feel
      foreach (ContactPoint2D contact in collision.contacts)
      {
        Vector2 normal = contact.normal;
        if (normal.y > 0.7f) // Only consider it ground if normal points mostly upward
        {
          groundContactCount++;
          if (!isGrounded)
          {
            // CRITICAL FIX: Multiple conditions to prevent immediate grounding
            bool hasMinimumAirTime = (Time.time - lastJumpTime) > minAirborneTime;
            bool isActuallyFalling = rb.linearVelocity.y <= 0.1f;
            bool notJustJumped = !isJumping || hasMinimumAirTime;

            if (isActuallyFalling && notJustJumped)
            {
              isGrounded = true;
              Debug.Log($"=== PLAYER GROUNDED ===");
              Debug.Log($"Y Velocity when grounded: {rb.linearVelocity.y}");
              Debug.Log($"Air time: {Time.time - lastJumpTime:F2}s");
              Debug.Log($"Contact normal: {normal}");

              // DEAD CELLS: Reset jump state when landing
              ResetJumpingState();

              // Trigger landing state transition if we were falling
              if (wasGroundedLastFrame == false && rb.linearVelocity.y < -1f)
              {
                // Let the state machine handle the landing transition
              }
            }
            else
            {
              Debug.Log($"=== GROUND CONTACT IGNORED ===");
              Debug.Log($"Y Velocity: {rb.linearVelocity.y}, Air time: {Time.time - lastJumpTime:F2}s");
              Debug.Log($"Min air time passed: {hasMinimumAirTime}, Actually falling: {isActuallyFalling}");
              Debug.Log($"Not just jumped: {notJustJumped}, Is jumping: {isJumping}");
            }
          }
          break;
        }
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.CompareTag("IceSpike"))
    {
      playerStats.TakeDamage(10f);
    }
    if (collision.gameObject.CompareTag("Water"))
    {
      GameManager.Instance.GameOver();
    }
    if (collision.gameObject.CompareTag("Trap"))
    {
      var damage = collision.GetComponent<BaseTrapStats>().GetDamage();
      playerStats.TakeDamage(damage);
    }
  }

  private void OnTriggerStay2D(Collider2D collision)
  {
    if (collision.gameObject.CompareTag("Trap"))
    {
      var damage = collision.GetComponent<BaseTrapStats>().GetDamage();
      playerStats.TakeDamage(damage);
    }
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Rock"))
    {
      groundContactCount--;
      if (groundContactCount <= 0)
      {
        groundContactCount = 0;
        isGrounded = false;
      }
    }
  }
}
