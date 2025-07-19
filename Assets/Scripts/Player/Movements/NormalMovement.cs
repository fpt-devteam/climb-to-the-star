using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Professional Dead Cells/Celeste/Hollow Knight Style Movement System
/// Implements all modern platformer best practices:
/// - Coyote Time, Jump Buffering, Variable Jump Height
/// - Anti-gravity at apex, Sticky feet, Speed apex
/// - Professional responsiveness and precision
/// </summary>
public class NormalMovement : IPlayerMovement
{
  private Transform transform;
  private Rigidbody2D rb;
  private PlayerStats playerStats;

  // === PROFESSIONAL MOVEMENT PARAMETERS ===
  [Header("Dead Cells Style Responsiveness")]
  private float groundAcceleration = 30f;    // Instant-feeling acceleration
  private float groundDeceleration = 40f;    // Snappy deceleration
  private float airAcceleration = 15f;       // Good air control
  private float airDeceleration = 8f;        // Maintain momentum
  private float directionChangeBoost = 2.5f; // Reactivity boost for direction changes

  [Header("Professional Jump System")]
  private float jumpForce = 12.5f;
  private float jumpCutMultiplier = 0.4f;    // How much to cut jump when released
  private float fallGravityMultiplier = 2.8f; // Faster falling for snappy feel
  private float lowJumpMultiplier = 2.2f;    // Variable jump height
  private float apexThreshold = 3f;          // Velocity threshold for apex detection
  private float apexGravityReduction = 0.6f; // Anti-gravity at apex

  [Header("Professional Input Timing")]
  private float coyoteTime = 0.12f;
  private float jumpBufferTime = 0.12f;
  private float coyoteTimeCounter = 0f;
  private float jumpBufferCounter = 0f;

  // === MOVEMENT STATE ===
  private Vector2 velocity;
  private Vector2 lastFrameVelocity;
  private bool isGrounded = false;
  private bool wasGroundedLastFrame = false;
  private bool isJumpCut = false;
  private bool wasMovingWhenJumped = false;
  private float lastGroundedTime = 0f;
  private float lastJumpTime = 0f;

  // === EXTERNAL CONTROL (Dash, Knockback, etc.) ===
  private bool isUnderExternalControl = false;
  private Vector2 externalVelocity = Vector2.zero;
  private float externalControlEndTime = 0f;

  // === PHYSICS ENHANCEMENT ===
  private bool isAtApex = false;
  private float currentGravityScale = 1f;
  private float baseGravityScale = 2.5f;

  // === TILEMAP COLLISION FIXES ===
  private Vector2 lastPosition = Vector2.zero;
  private Vector2 lastVelocity = Vector2.zero;
  private int stuckFrameCount = 0;
  private const int maxStuckFrames = 5; // Allow 5 frames before considering stuck
  private const float stuckVelocityThreshold = 0.1f; // Minimum velocity to consider "moving"
  private const float unstuckForce = 2f; // Force to apply when unsticking

  public void Initialize(PlayerStats playerStats)
  {
    this.playerStats = playerStats;
    this.rb = playerStats.GetComponent<Rigidbody2D>();
    this.transform = playerStats.transform;

    // Get values from PlayerStats
    jumpForce = playerStats.JumpForce;
    baseGravityScale = playerStats.GravityScale;

    // Calculate professional movement parameters based on stats
    groundAcceleration = playerStats.MoveSpeed * 4f;   // Reduced from 8f for smoother movement
    groundDeceleration = playerStats.MoveSpeed * 6f;   // Reduced from 10f for less aggressive stops
    airAcceleration = playerStats.MoveSpeedInAir * 3f; // Reduced from 5f for smoother air control
    airDeceleration = playerStats.MoveSpeedInAir * 2f; // Reduced from 3f for better air flow

    // Professional physics setup
    SetupPhysics();

    // Initialize state
    velocity = rb.linearVelocity;
    lastFrameVelocity = velocity;
    isGrounded = false;
    wasGroundedLastFrame = false;

    Debug.Log("Professional Movement System Initialized - Dead Cells/Celeste Style");
  }

  private void SetupPhysics()
  {
    rb.gravityScale = baseGravityScale;
    rb.freezeRotation = true;

    // CRITICAL FIX: Use Continuous collision detection to prevent tilemap sticking
    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

    // CRITICAL FIX: Use interpolation for smooth movement
    rb.interpolation = RigidbodyInterpolation2D.Interpolate;

    rb.linearDamping = 0f; // We handle all damping manually
    rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    // CRITICAL FIX: Use discrete sleeping for better responsiveness
    rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

    // TILEMAP FIX: Create zero-friction physics material to prevent sticking
    PhysicsMaterial2D frictionlessMaterial = new PhysicsMaterial2D("PlayerFrictionless");
    frictionlessMaterial.friction = 0f; // No friction
    frictionlessMaterial.bounciness = 0f; // No bounce

    // Apply material to collider
    Collider2D collider = playerStats.GetComponent<Collider2D>();
    if (collider != null)
    {
      collider.sharedMaterial = frictionlessMaterial;
      Debug.Log("Applied frictionless physics material to prevent tilemap sticking");
    }

    // We control X velocity manually, let physics handle Y for gravity
    currentGravityScale = baseGravityScale;

    Debug.Log("Physics setup with tilemap compatibility fixes applied");
  }

  public void Move(float direction)
  {
    // Don't override external control (dash, knockback, etc.)
    if (isUnderExternalControl && Time.time < externalControlEndTime)
    {
      ApplyExternalMovement();
      Debug.Log($"Move blocked - under external control until {externalControlEndTime}, current time: {Time.time}");
      return;
    }

    // Clear external control if expired
    if (isUnderExternalControl && Time.time >= externalControlEndTime)
    {
      ClearExternalControl();
    }

    // TILEMAP FIX: Detect if we're stuck and unstick
    DetectAndFixStuckMovement(direction);

    // Professional horizontal movement
    ApplyHorizontalMovement(direction);

    // Apply enhanced physics
    ApplyEnhancedPhysics();

    // Update timers
    UpdateMovementTimers();
  }

  private void ApplyHorizontalMovement(float direction)
  {
    float targetSpeed = direction * (isGrounded ? playerStats.MoveSpeed : playerStats.MoveSpeedInAir);
    float currentSpeed = rb.linearVelocity.x; // Use rigidbody velocity directly

    // ENHANCED MOMENTUM PRESERVATION: Maintain at least ground speed in air if moving in same direction
    if (!isGrounded && Mathf.Abs(direction) > 0.1f && Mathf.Sign(direction) == Mathf.Sign(currentSpeed))
    {
      float minAirSpeed = direction * playerStats.MoveSpeed; // Use ground speed as minimum in air
      if (Mathf.Abs(targetSpeed) < Mathf.Abs(minAirSpeed))
      {
        targetSpeed = minAirSpeed;
        Debug.Log($"Enhanced air momentum: Using ground speed {minAirSpeed} instead of air speed {direction * playerStats.MoveSpeedInAir}");
      }
    }

    // MOMENTUM PRESERVATION: Don't slow down existing momentum in air unless changing direction
    if (!isGrounded && Mathf.Sign(direction) == Mathf.Sign(currentSpeed) && Mathf.Abs(currentSpeed) > Mathf.Abs(targetSpeed))
    {
      // Preserve higher momentum from ground movement when jumping
      targetSpeed = direction * Mathf.Abs(currentSpeed);
      Debug.Log($"Preserving air momentum: {targetSpeed} (was going to reduce to {direction * playerStats.MoveSpeedInAir})");
    }

    // Professional acceleration/deceleration
    float acceleration = isGrounded ? groundAcceleration : airAcceleration;
    float deceleration = isGrounded ? groundDeceleration : airDeceleration;

    // DEAD CELLS: Enhanced reactivity for direction changes
    bool isChangingDirection = (direction > 0 && currentSpeed < 0) || (direction < 0 && currentSpeed > 0);
    if (isChangingDirection && Mathf.Abs(direction) > 0.1f)
    {
      acceleration *= directionChangeBoost; // Instant direction changes
    }

    // Calculate new speed with momentum preservation
    float newXVelocity;
    if (Mathf.Abs(direction) > 0.1f)
    {
      // MOMENTUM PRESERVATION: Only accelerate towards target if it's higher than current, or if changing direction
      if (isChangingDirection || Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
      {
        newXVelocity = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
      }
      else
      {
        // Maintain current speed if it's already higher than target (preserve momentum)
        newXVelocity = currentSpeed;
      }
    }
    else
    {
      // No input - decelerate (but more gently in air)
      float actualDeceleration = isGrounded ? deceleration : deceleration * 0.3f; // Slower air deceleration
      newXVelocity = Mathf.MoveTowards(currentSpeed, 0f, actualDeceleration * Time.fixedDeltaTime);
    }

    // PURE RIGIDBODY MOVEMENT: Apply directly to rigidbody for smoothness
    rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);

    // Update our internal velocity tracking
    velocity = rb.linearVelocity;
  }

  private void ApplyEnhancedPhysics()
  {
    // Update velocity to current rigidbody velocity
    velocity = rb.linearVelocity;

    // PROFESSIONAL: Anti-gravity at jump apex for better air control
    bool wasAtApex = isAtApex;
    isAtApex = !isGrounded && Mathf.Abs(velocity.y) < apexThreshold && velocity.y > 0;

    if (isAtApex && !wasAtApex)
    {
      // Entering apex - reduce gravity for "hang time"
      currentGravityScale = baseGravityScale * apexGravityReduction;
      rb.gravityScale = currentGravityScale;
      Debug.Log("Apex detected - reducing gravity for hang time");
    }
    else if (!isAtApex && wasAtApex)
    {
      // Leaving apex - restore normal gravity
      RestoreNormalGravity();
    }

    // DEAD CELLS: Enhanced falling gravity for snappy feel
    if (velocity.y < 0 && !isAtApex)
    {
      currentGravityScale = baseGravityScale * fallGravityMultiplier;
      rb.gravityScale = currentGravityScale;
    }

    // PROFESSIONAL: Variable jump height - cut jump when released
    if (isJumpCut && velocity.y > 0)
    {
      currentGravityScale = baseGravityScale * lowJumpMultiplier;
      rb.gravityScale = currentGravityScale;
    }

    // Clamp maximum fall speed for control
    if (velocity.y < -playerStats.MaxFallSpeed)
    {
      rb.linearVelocity = new Vector2(rb.linearVelocity.x, -playerStats.MaxFallSpeed);
      velocity = rb.linearVelocity; // Update our tracking
    }

    lastFrameVelocity = velocity;
  }

  private void RestoreNormalGravity()
  {
    currentGravityScale = baseGravityScale;
    rb.gravityScale = currentGravityScale;
  }

  private void UpdateMovementTimers()
  {
    // Track grounded state for timers
    wasGroundedLastFrame = isGrounded;

    // Update last grounded time
    if (isGrounded)
    {
      lastGroundedTime = Time.time;
    }
  }

  public void MovementInAir(float direction)
  {
    // Enhanced air movement with better control
    Move(direction);
  }

  public void Jump()
  {
    isJumpCut = false;

    // MOMENTUM PRESERVATION: Use rigidbody velocity directly for perfect sync
    float jumpMomentumX = rb.linearVelocity.x;
    wasMovingWhenJumped = Mathf.Abs(jumpMomentumX) > 0.5f;

    float currentYVelocity = rb.linearVelocity.y;
    float finalJumpForce = jumpForce;

    // Double jump compensation
    if (currentYVelocity < 2f)
    {
      finalJumpForce += Mathf.Abs(currentYVelocity) * 0.2f;
      Debug.Log($"Double jump compensation added: {Mathf.Abs(currentYVelocity) * 0.2f}");
    }

    // PURE RIGIDBODY: Apply jump while perfectly preserving horizontal momentum
    rb.linearVelocity = new Vector2(jumpMomentumX, finalJumpForce);

    // Update our internal tracking
    velocity = rb.linearVelocity;

    coyoteTimeCounter = 0f;
    jumpBufferCounter = 0f;
    lastJumpTime = Time.time;

    RestoreNormalGravity();

    Debug.Log($"PURE RIGIDBODY JUMP - Force: {finalJumpForce}, X momentum: {jumpMomentumX} (was moving: {wasMovingWhenJumped})");
  }

  public void CutJump()
  {
    if (!isJumpCut && rb.linearVelocity.y > 0)
    {
      isJumpCut = true;
      rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
      Debug.Log("Jump cut applied for variable jump height");
    }
  }

  public void Fall()
  {
    // Enhanced falling physics already handled in ApplyEnhancedPhysics()
    // This method kept for interface compatibility
  }

  // === COYOTE TIME & JUMP BUFFERING ===
  public void UpdateCoyoteTime(bool grounded)
  {
    isGrounded = grounded;

    // Professional coyote time implementation
    if (isGrounded)
    {
      coyoteTimeCounter = coyoteTime;

      // Reset jump states when landing (simplified)
      if (!wasGroundedLastFrame)
      {
        isJumpCut = false;
        isAtApex = false;
        RestoreNormalGravity();
        Debug.Log("Landing detected - jump states reset");
      }
    }
    else
    {
      coyoteTimeCounter -= Time.deltaTime;
    }
  }

  public void UpdateJumpBuffer(bool jumpPressed)
  {
    if (jumpPressed)
    {
      jumpBufferCounter = jumpBufferTime;
    }
    else if (jumpBufferCounter > 0f)
    {
      jumpBufferCounter -= Time.deltaTime;
    }
  }

  public bool CanCoyoteJump()
  {
    return coyoteTimeCounter > 0f;
  }

  public bool HasBufferedJump()
  {
    return jumpBufferCounter > 0f;
  }

  // === EXTERNAL CONTROL (Dash, Knockback) ===
  public void SetExternalVelocityControl(float duration)
  {
    isUnderExternalControl = true;
    externalControlEndTime = Time.time + duration;
    Debug.Log($"External velocity control set for {duration}s until {externalControlEndTime}");
  }

  public void SetExternalVelocity(Vector2 newVelocity)
  {
    externalVelocity = newVelocity;
    rb.linearVelocity = externalVelocity;
    Debug.Log($"External velocity set to: {externalVelocity}");
  }

  private void ApplyExternalMovement()
  {
    rb.linearVelocity = externalVelocity;
  }

  private void ClearExternalControl()
  {
    isUnderExternalControl = false;
    externalVelocity = Vector2.zero;
    Debug.Log("External velocity control cleared");
  }

  public void ForceStopExternalControl()
  {
    ClearExternalControl();
    Debug.Log("External control force stopped");
  }

  // === STATE QUERIES ===
  public bool IsUnderExternalControl()
  {
    return isUnderExternalControl && Time.time < externalControlEndTime;
  }

  public bool IsMovingHorizontally()
  {
    return Mathf.Abs(rb.linearVelocity.x) > 0.2f; // Use rigidbody velocity directly
  }

  public bool IsJumpCut()
  {
    return isJumpCut;
  }

  public Vector2 GetVelocity()
  {
    return rb.linearVelocity; // Return rigidbody velocity directly
  }

  public bool IsGrounded()
  {
    return isGrounded;
  }

  public bool IsAtJumpApex()
  {
    return isAtApex;
  }

  // === TILEMAP COLLISION FIXES ===
  private void DetectAndFixStuckMovement(float direction)
  {
    Vector2 currentPosition = transform.position;
    Vector2 currentVelocity = rb.linearVelocity;

    // Check if player is trying to move but not actually moving
    bool isTryingToMove = Mathf.Abs(direction) > 0.1f;
    bool isActuallyMoving = Mathf.Abs(currentVelocity.x) > stuckVelocityThreshold;
    bool hasMovedPosition = Vector2.Distance(currentPosition, lastPosition) > 0.01f;

    if (isTryingToMove && !isActuallyMoving && !hasMovedPosition && isGrounded)
    {
      stuckFrameCount++;
      Debug.Log($"Potential stuck detection: Frame {stuckFrameCount}/{maxStuckFrames}, Input: {direction}, Vel: {currentVelocity.x:F3}");

      if (stuckFrameCount >= maxStuckFrames)
      {
        // Player is stuck! Apply unstuck fix
        UnstickPlayer(direction);
        stuckFrameCount = 0;
      }
    }
    else
    {
      // Reset stuck counter if player is moving normally
      stuckFrameCount = 0;
    }

    // Update tracking
    lastPosition = currentPosition;
    lastVelocity = currentVelocity;
  }

  private void UnstickPlayer(float direction)
  {
    Debug.Log($"TILEMAP STUCK DETECTED - Applying unstick fix! Direction: {direction}");

    // Method 1: Apply small upward force to get unstuck from tilemap edges
    Vector2 unstuckVector = new Vector2(direction * unstuckForce, unstuckForce * 0.5f);
    rb.linearVelocity = unstuckVector;

    // Method 2: Slightly move player position to avoid collision overlap
    Vector2 currentPos = transform.position;
    Vector2 unstickOffset = new Vector2(direction * 0.05f, 0.05f); // Very small offset
    transform.position = currentPos + unstickOffset;

    Debug.Log($"Applied unstick: velocity={unstuckVector}, position offset={unstickOffset}");

    // Log for debugging
    Debug.Log("Player was stuck on tilemap collider - unsticking applied");
  }
}
