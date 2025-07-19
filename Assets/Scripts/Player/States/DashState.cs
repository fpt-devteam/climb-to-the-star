using System.Collections;
using UnityEngine;

/// <summary>
/// Professional Celeste-Style 8-Directional Dash System
/// GROUND: Left/Right only (4 directions including diagonals)
/// AIR: Full 8-directional freedom
/// Features: Input buffering, ghost trail, invincibility, momentum preservation
/// </summary>
public class DashState : BasePlayerState
{
  [Header("Celeste Dash System")]
  [SerializeField] private float dashSpeed = 35f;
  [SerializeField] private float dashDuration = 0.15f;        // Quick, snappy dash
  [SerializeField] private float dashCooldown = 0.1f;         // Balanced cooldown
  [SerializeField] private float dashInvincibilityDuration = 0.2f; // Slightly longer than dash

  [Header("8-Direction Air Dash")]
  [SerializeField] private float airDashSpeedMultiplier = 1.1f; // Slightly faster in air
  [SerializeField] private bool allowDiagonalGroundDash = true;  // Enable diagonal ground dashes

  [Header("Input & Feel")]
  [SerializeField] private float inputBufferTime = 0.1f;       // Direction input buffer
  [SerializeField] private float momentumPreservation = 0.3f;  // How much momentum to keep after dash
  [SerializeField] private float endLagDuration = 0.05f;       // Brief end lag for control

  // Components
  private Animator animator;
  private Rigidbody2D rb;
  private BoxCollider2D boxCollider;
  private TrailRenderer trailRenderer;
  private PlayerGhostTrail ghostTrail;
  private IPlayerMovement movement;
  private IPlayerInput input;

  // State tracking
  private Vector2 dashDirection = Vector2.zero;
  private float dashTimer = 0f;
  private float currentDashSpeed = 0f;
  private bool dashCompleted = false;
  private bool isInEndLag = false;
  private float endLagTimer = 0f;

  // Input buffering for direction
  private Vector2 bufferedDirection = Vector2.zero;
  private float directionBufferTimer = 0f;

  // Dash restrictions
  private bool isGrounded = false;
  private LayerMask originalExcludeLayers;

  public DashState(PlayerController context) : base(context)
  {
    animator = context.GetComponent<Animator>();
    rb = context.GetComponent<Rigidbody2D>();
    boxCollider = context.GetComponent<BoxCollider2D>();
    trailRenderer = context.GetComponentInChildren<TrailRenderer>();
    ghostTrail = context.GetComponent<PlayerGhostTrail>();
    movement = context.PlayerMovement;
    input = context.PlayerInput;

    // Store original collision settings
    originalExcludeLayers = boxCollider.excludeLayers;
  }

  public override void Enter()
  {
    // Reset state
    dashTimer = 0f;
    endLagTimer = 0f;
    dashCompleted = false;
    isInEndLag = false;
    isGrounded = context.IsGrounded;

    // Start cooldown immediately
    context.StartDashCooldown();

    // Determine dash direction based on Celeste rules
    dashDirection = GetDashDirection();

    // Calculate dash speed (air dash slightly faster)
    currentDashSpeed = isGrounded ? dashSpeed : dashSpeed * airDashSpeedMultiplier;

    // Set up movement control
    movement.SetExternalVelocityControl(dashDuration + endLagDuration + 0.05f);
    movement.SetExternalVelocity(dashDirection * currentDashSpeed);

    // Visual and audio feedback
    SetupDashEffects();

    // Enable invincibility
    EnableDashInvincibility();

    // Animation
    PlayDashAnimation();

    Debug.Log($"=== CELESTE DASH START ===");
    Debug.Log($"Direction: {dashDirection}, Speed: {currentDashSpeed}");
    Debug.Log($"Grounded: {isGrounded}, 8-Dir Available: {!isGrounded}");
    Debug.Log($"Duration: {dashDuration}s");
  }

  private Vector2 GetDashDirection()
  {
    // Get current input direction
    Vector2 inputDirection = GetInputDirection();

    // Check for buffered direction first
    if (directionBufferTimer > 0f && bufferedDirection != Vector2.zero)
    {
      inputDirection = bufferedDirection;
      Debug.Log($"Using buffered direction: {bufferedDirection}");
    }

    // CELESTE RULES: Ground vs Air directional restrictions
    if (isGrounded)
    {
      return GetGroundDashDirection(inputDirection);
    }
    else
    {
      return GetAirDashDirection(inputDirection);
    }
  }

  private Vector2 GetGroundDashDirection(Vector2 input)
  {
    // GROUND DASH: Primarily horizontal with optional diagonals

    // Pure horizontal (most common)
    if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.3f)
    {
      return new Vector2(Mathf.Sign(input.x), 0f).normalized;
    }

    // Diagonal ground dashes (if enabled)
    if (allowDiagonalGroundDash && input.magnitude > 0.5f)
    {
      // Allow slight upward diagonals
      if (input.y > 0.3f && Mathf.Abs(input.x) > 0.1f)
      {
        return new Vector2(Mathf.Sign(input.x), 0.6f).normalized; // Shallow upward diagonal
      }
    }

    // No input or invalid - use facing direction
    return new Vector2(context.IsFacingRight ? 1f : -1f, 0f);
  }

  private Vector2 GetAirDashDirection(Vector2 input)
  {
    // AIR DASH: Full 8-directional freedom

    if (input.magnitude > 0.1f)
    {
      // Snap to 8 cardinal/diagonal directions for consistency
      Vector2 direction = input.normalized;

      // Determine the closest 8-direction
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      float snappedAngle = Mathf.Round(angle / 45f) * 45f;

      Vector2 snappedDirection = new Vector2(
          Mathf.Cos(snappedAngle * Mathf.Deg2Rad),
          Mathf.Sin(snappedAngle * Mathf.Deg2Rad)
      );

      Debug.Log($"Air dash - Input angle: {angle:F1}°, Snapped: {snappedAngle:F1}°");
      return snappedDirection.normalized;
    }

    // No input - dash in facing direction
    return new Vector2(context.IsFacingRight ? 1f : -1f, 0f);
  }

  private Vector2 GetInputDirection()
  {
    // Use the enhanced input system for cleaner directional input
    return input.GetDirectionalInput();
  }

  private void SetupDashEffects()
  {
    // Ghost trail
    if (ghostTrail != null)
    {
      ghostTrail.setSpawnGhost(true);
    }

    // Trail renderer
    if (trailRenderer != null)
    {
      trailRenderer.Clear();
      trailRenderer.enabled = true;
      trailRenderer.emitting = true;
    }

    // Audio
    AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump); // Use existing jump sound for now
  }

  private void EnableDashInvincibility()
  {
    // Enable dash invincibility (pass through enemies)
    context.PlayerStats.SetDashInvincibility(dashInvincibilityDuration);
    boxCollider.excludeLayers = LayerMask.GetMask("Enemy");

    Debug.Log($"Dash invincibility enabled for {dashInvincibilityDuration}s");
  }

  private void PlayDashAnimation()
  {
    // Choose animation based on direction
    string animationName = "Dash";

    // You could have directional dash animations
    if (!isGrounded && Mathf.Abs(dashDirection.y) > 0.5f)
    {
      animationName = dashDirection.y > 0 ? "DashUp" : "DashDown";
    }

    animator.Play(animationName);
  }

  public override void Update()
  {
    // Buffer direction input for more forgiving timing
    UpdateDirectionBuffer();

    // Update dash timing
    UpdateDashTiming();
  }

  private void UpdateDirectionBuffer()
  {
    Vector2 currentInput = GetInputDirection();

    if (currentInput.magnitude > 0.1f)
    {
      bufferedDirection = currentInput;
      directionBufferTimer = inputBufferTime;
    }
    else if (directionBufferTimer > 0f)
    {
      directionBufferTimer -= Time.deltaTime;
    }
  }

  private void UpdateDashTiming()
  {
    if (!dashCompleted)
    {
      dashTimer += Time.deltaTime;

      if (dashTimer >= dashDuration)
      {
        CompleteDash();
      }
    }
    else if (isInEndLag)
    {
      endLagTimer += Time.deltaTime;

      if (endLagTimer >= endLagDuration)
      {
        CompleteEndLag();
      }
    }
  }

  public override void FixedUpdate()
  {
    if (!dashCompleted)
    {
      // Maintain dash velocity
      movement.SetExternalVelocity(dashDirection * currentDashSpeed);
    }
    else if (isInEndLag)
    {
      // Gradual momentum preservation during end lag
      float preservedSpeed = currentDashSpeed * momentumPreservation;
      Vector2 preservedVelocity = dashDirection * preservedSpeed;
      movement.SetExternalVelocity(preservedVelocity);
    }
  }

  private void CompleteDash()
  {
    dashCompleted = true;
    isInEndLag = true;
    endLagTimer = 0f;

    // Disable effects
    DisableDashEffects();

    Debug.Log("Dash completed - entering end lag");
  }

  private void CompleteEndLag()
  {
    isInEndLag = false;

    // Restore collision settings
    boxCollider.excludeLayers = originalExcludeLayers;

    Debug.Log("End lag completed - dash state finished");
  }

  private void DisableDashEffects()
  {
    // Ghost trail
    if (ghostTrail != null)
    {
      ghostTrail.setSpawnGhost(false);
    }

    // Trail renderer fade
    if (trailRenderer != null)
    {
      trailRenderer.emitting = false;
    }
  }

  public override IState CheckTransitions()
  {
    // Can't transition during active dash
    if (!dashCompleted || isInEndLag)
    {
      return null;
    }

    // CRITICAL FIX: After dash completion, ensure external control is cleared
    if (!movement.IsUnderExternalControl())
    {
      Debug.Log("Dash state complete - external control already cleared, ready for transitions");
    }
    else
    {
      Debug.Log("Dash state complete but external control still active - forcing clear");
      movement.ForceStopExternalControl();
    }

    // After dash completion, check for other actions

    // High priority actions (can interrupt dash momentum)
    if (context.IsJumping())
    {
      Debug.Log("DashState -> AirState (jump input)");
      return context.GetState(PlayerState.Air);
    }

    if (context.CanAttack())
    {
      Debug.Log("DashState -> Attack1 (attack input)");
      return context.GetState(PlayerState.Attack1);
    }

    // Check current grounded state for correct transition
    bool isCurrentlyGrounded = context.IsGrounded;
    Debug.Log($"DashState ending - IsGrounded: {isCurrentlyGrounded}");

    if (isCurrentlyGrounded)
    {
      Debug.Log("DashState -> Locomotion (grounded)");
      return context.GetState(PlayerState.Locomotion);
    }
    else
    {
      Debug.Log("DashState -> AirState (not grounded)");
      return context.GetState(PlayerState.Air);
    }
  }

  public override void Exit()
  {
    // Cleanup
    DisableDashEffects();

    // Restore collision settings
    boxCollider.excludeLayers = originalExcludeLayers;

    // Clear direction buffer
    bufferedDirection = Vector2.zero;
    directionBufferTimer = 0f;

    Debug.Log("=== DASH STATE EXIT ===");
  }

  // === DASH CANCELING (Dead Cells style) ===
  public bool CanDashCancel()
  {
    // Allow dash canceling after a portion of the dash
    return dashTimer >= dashDuration * 0.6f;
  }

  public void CancelDash()
  {
    if (CanDashCancel())
    {
      CompleteDash();
      Debug.Log("Dash canceled via dash cancel");
    }
  }
}
