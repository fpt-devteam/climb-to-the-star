using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Transform-based movement system like Dead Cells/Hollow Knight
public class NormalMovement : IPlayerMovement
{
  private Transform transform;
  private Rigidbody2D rb;
  private PlayerStats playerStats;

  // Direct movement control (like Dead Cells/Hollow Knight)
  private float currentVelocityX = 0f;
  private float currentVelocityY = 0f;
  private float velocitySmoothing = 0f;

  // Movement parameters
  private float acceleration;
  private float deceleration;
  private float airAcceleration;
  private float airDeceleration;

  // Jump enhancement variables
  private float coyoteTimeCounter;
  private float jumpBufferCounter;
  private bool isJumpCut;
  private float lastGroundedTime;
  private float lastJumpPressedTime;

  // Movement state tracking
  private bool wasMovingWhenJumped;
  private float jumpStartHorizontalSpeed;
  private Vector2 landingVelocity;

  // External control state
  private bool isExternalVelocityControl = false;
  private float externalVelocityEndTime = 0f;
  private Vector2 externalVelocity = Vector2.zero;

  public void Initialize(PlayerStats playerStats)
  {
    this.playerStats = playerStats;
    this.rb = playerStats.GetComponent<Rigidbody2D>();
    this.transform = playerStats.transform;

    // Dead Cells/Hollow Knight style movement parameters
    acceleration = playerStats.MoveSpeed * 25f; // Very fast acceleration
    deceleration = playerStats.MoveSpeed * 30f; // Instant-feeling deceleration
    airAcceleration = playerStats.MoveSpeedInAir * 12f;
    airDeceleration = playerStats.MoveSpeedInAir * 8f;

    // Initialize movement state
    currentVelocityX = 0f;
    currentVelocityY = 0f;
    velocitySmoothing = 0f;

    // Initialize counters
    coyoteTimeCounter = 0f;
    jumpBufferCounter = 0f;
    isJumpCut = false;
    lastGroundedTime = 0f;
    lastJumpPressedTime = 0f;

    // Movement state
    wasMovingWhenJumped = false;
    jumpStartHorizontalSpeed = 0f;
    landingVelocity = Vector2.zero;

    // Set up Rigidbody2D for jumping only (not movement)
    rb.gravityScale = playerStats.GravityScale;
    rb.freezeRotation = true;
    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    rb.linearDamping = 0f;

    // We'll manually control X movement, let physics handle Y for jumps/gravity
    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
  }

  public void Move(float direction)
  {
    Debug.Log($"Move called - direction: {direction}, isExternalControl: {isExternalVelocityControl}, timeCheck: {Time.time < externalVelocityEndTime}, endTime: {externalVelocityEndTime}");

    // Don't override external velocity control (dash, knockback, etc.)
    if (isExternalVelocityControl && Time.time < externalVelocityEndTime)
    {
      Debug.Log("Move: Calling ApplyExternalMovement");
      ApplyExternalMovement();
      return;
    }

    // Clear external control if expired
    if (isExternalVelocityControl && Time.time >= externalVelocityEndTime)
    {
      Debug.Log("Move: External control expired, clearing");
      isExternalVelocityControl = false;
      externalVelocity = Vector2.zero;
    }

    // Dead Cells/Hollow Knight style: Direct velocity control
    float targetVelocityX = direction * playerStats.MoveSpeed;

    // INSTANT STOP when no input (like Dead Cells)
    if (Mathf.Abs(direction) < 0.01f)
    {
      currentVelocityX = 0f; // Absolute zero
    }
    else
    {
      // Instant direction changes and acceleration
      if (Mathf.Sign(direction) != Mathf.Sign(currentVelocityX) && Mathf.Abs(currentVelocityX) > 0.1f)
      {
        // Instant turnaround like Dead Cells
        currentVelocityX = targetVelocityX * 0.6f;
      }
      else
      {
        // Smooth acceleration to target speed
        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocityX, acceleration * Time.fixedDeltaTime);
      }
    }

    // Apply horizontal movement directly to transform (like professional games)
    Vector3 movement = Vector3.right * currentVelocityX * Time.fixedDeltaTime;
    transform.position += movement;

    // Keep rigidbody X velocity in sync for physics interactions
    rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);
  }

  public void MovementInAir(float direction)
  {
    // Don't override external control
    if (isExternalVelocityControl && Time.time < externalVelocityEndTime)
    {
      ApplyExternalMovement();
      return;
    }

    // Celeste-style air movement with momentum preservation
    float targetVelocityX = direction * playerStats.MoveSpeedInAir;

    // Air movement with different rules
    if (Mathf.Abs(direction) < 0.01f)
    {
      // Slight air friction when no input
      currentVelocityX = Mathf.MoveTowards(currentVelocityX, 0f, airDeceleration * 0.3f * Time.fixedDeltaTime);
    }
    else
    {
      // Air control acceleration
      float accelRate;

      if (Mathf.Sign(direction) != Mathf.Sign(currentVelocityX))
      {
        // Direction change in air - slower
        accelRate = airAcceleration * 0.7f;
      }
      else
      {
        // Same direction
        accelRate = airAcceleration;
      }

      currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocityX, accelRate * Time.fixedDeltaTime);
    }

    // Apply air movement
    Vector3 movement = Vector3.right * currentVelocityX * Time.fixedDeltaTime;
    transform.position += movement;

    // Sync with rigidbody
    rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);
  }

  public void Jump()
  {
    // Track movement state when jumping
    wasMovingWhenJumped = Mathf.Abs(currentVelocityX) > 1f;
    jumpStartHorizontalSpeed = Mathf.Abs(currentVelocityX);

    // Reset jump cut flag
    isJumpCut = false;

    // Use rigidbody for vertical movement (jumping/gravity)
    float jumpVelocity = playerStats.JumpForce;

    // FIXED: Reduce jump force when moving fast horizontally (like after dash)
    float horizontalSpeedFactor = Mathf.Abs(currentVelocityX);
    if (horizontalSpeedFactor > 15f) // If moving very fast (like after dash)
    {
      jumpVelocity *= 0.85f; // Reduce jump force by 15%
      Debug.Log($"High speed jump - reduced force by 15%");
    }

    // ENHANCED: Much stronger double jump when falling or moving slow vertically
    if (rb.linearVelocity.y < 2f) // If falling or barely rising
    {
      // Add significant compensation for double jump feel
      float fallCompensation = Mathf.Abs(rb.linearVelocity.y) * 0.4f; // Reduced from 0.8f

      // Additional base compensation for double jumps
      if (rb.linearVelocity.y < 0f) // Only if actually falling
      {
        fallCompensation += 1.5f; // Reduced from 3f for more balanced double jump
      }

      jumpVelocity += fallCompensation;
      Debug.Log($"Double jump compensation - added {fallCompensation:F1} extra force");
    }

    Debug.Log($"=== JUMP METHOD CALLED ===");
    Debug.Log($"Current Y Velocity: {rb.linearVelocity.y}");
    Debug.Log($"Horizontal Speed: {horizontalSpeedFactor:F1}");
    Debug.Log($"Base Jump Force: {playerStats.JumpForce}");
    Debug.Log($"Final Jump Force Applied: {jumpVelocity}");

    // CRITICAL FIX: Use ONLY rigidbody for jump physics, no transform manipulation
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

    Debug.Log($"Final Velocity: ({rb.linearVelocity.x}, {rb.linearVelocity.y})");

    coyoteTimeCounter = 0f;
  }

  public void Fall()
  {
    // Enhanced falling with variable gravity
    float fallMultiplier = playerStats.FallGravityMultiplier;
    float lowJumpMultiplier = playerStats.LowJumpMultiplier;

    if (rb.linearVelocity.y < 0)
    {
      rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }
    else if (rb.linearVelocity.y > 0 && isJumpCut)
    {
      rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    // Clamp fall speed
    if (rb.linearVelocity.y < -playerStats.MaxFallSpeed)
    {
      rb.linearVelocity = new Vector2(rb.linearVelocity.x, -playerStats.MaxFallSpeed);
    }

    // Keep our controlled X velocity in sync
    rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);
  }

  // External velocity control for dash, knockback, etc.
  public void SetExternalVelocityControl(float duration)
  {
    isExternalVelocityControl = true;
    externalVelocityEndTime = Time.time + duration;
  }

  public void SetExternalVelocity(Vector2 velocity)
  {
    externalVelocity = velocity;
    currentVelocityX = velocity.x; // Update our internal state
  }

  public void ClearExternalVelocityControl()
  {
    isExternalVelocityControl = false;
    externalVelocityEndTime = 0f;
    externalVelocity = Vector2.zero;
  }

  public bool IsUnderExternalControl()
  {
    return isExternalVelocityControl && Time.time < externalVelocityEndTime;
  }

  private void ApplyExternalMovement()
  {
    // Apply external movement (like dash) directly to transform
    Vector3 movement = (Vector3)externalVelocity * Time.fixedDeltaTime;
    Vector3 oldPosition = transform.position;
    transform.position += movement;

    Debug.Log($"EXTERNAL MOVEMENT: velocity={externalVelocity}, movement={movement}, oldPos={oldPosition:F2}, newPos={transform.position:F2}");

    // Keep rigidbody in sync
    rb.linearVelocity = new Vector2(externalVelocity.x, rb.linearVelocity.y);
  }

  // Enhanced platformer features
  public void UpdateCoyoteTime(bool isGrounded)
  {
    if (isGrounded)
    {
      coyoteTimeCounter = playerStats.CoyoteTime;
      lastGroundedTime = Time.time;
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
      jumpBufferCounter = playerStats.JumpBufferTime;
      lastJumpPressedTime = Time.time;
    }
    else
    {
      jumpBufferCounter -= Time.deltaTime;
    }
  }

  public bool CanCoyoteJump()
  {
    return coyoteTimeCounter > 0f;
  }

  public bool HasJumpBuffer()
  {
    return jumpBufferCounter > 0f;
  }

  public void CutJump()
  {
    isJumpCut = true;
  }

  public bool IsJumpCut()
  {
    return isJumpCut;
  }

  // Landing mechanics
  public void OnLanding()
  {
    landingVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

    // CRITICAL FIX: Always clear external control on landing to prevent stuck states
    Debug.Log("OnLanding: Clearing external velocity control");
    ClearExternalVelocityControl();
  }

  public bool ShouldRollOnLanding()
  {
    return wasMovingWhenJumped || Mathf.Abs(landingVelocity.x) > 3f;
  }

  public float GetLandingHorizontalSpeed()
  {
    return Mathf.Abs(landingVelocity.x);
  }

  // Movement state queries
  public float GetHorizontalSpeed()
  {
    return Mathf.Abs(currentVelocityX);
  }

  public bool IsMovingHorizontally()
  {
    // Match the stopping precision for accurate state detection
    return Mathf.Abs(currentVelocityX) > 0.01f;
  }

  // CRITICAL FIX: Add method to force clear external control when stuck
  public void ForceStopExternalControl()
  {
    Debug.Log("ForceStopExternalControl: Emergency clearing of external velocity control");
    isExternalVelocityControl = false;
    externalVelocityEndTime = 0f;
    externalVelocity = Vector2.zero;
  }

  // Get current velocities for debugging
  public Vector2 GetCurrentVelocity()
  {
    return new Vector2(currentVelocityX, rb.linearVelocity.y);
  }
}
