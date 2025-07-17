using System.Collections;
using UnityEngine;

public class AirState : BasePlayerState
{
  private Animator animator;
  private IPlayerMovement movement;
  private IPlayerInput input;
  private Rigidbody2D rb;

  // Air state management
  private bool hasAppliedJump = false;
  private bool isRising = false;
  private bool isFalling = false;
  private float airTimer = 0f;
  private float lastJumpTime = 0f;
  private float doubleJumpGraceTime = 0.2f; // Time window for clean double jump
  private float maxAirTime = 5f; // Safety timeout to prevent infinite falling

  public AirState(PlayerController context) : base(context)
  {
    animator = context.GetComponent<Animator>();
    movement = context.PlayerMovement;
    input = context.PlayerInput;
    rb = context.GetComponent<Rigidbody2D>();
  }

  public override void Enter()
  {
    Debug.Log($"=== ENTERED AIR STATE ===");
    Debug.Log($"Current Jumps: {context.GetCurrentJumps()}, Y Velocity: {rb.linearVelocity.y}");
    Debug.Log($"Is Grounded: {context.IsGrounded}, Was this a jump: {context.IsJumping()}");
    Debug.Log($"Under external control: {movement.IsUnderExternalControl()}");

    airTimer = 0f;
    hasAppliedJump = false;

    // CRITICAL FIX: If entering AirState but stuck under external control, clear it
    if (movement.IsUnderExternalControl() && !context.IsJumping())
    {
      Debug.Log("AirState: Clearing stuck external control on air entry");
      movement.ForceStopExternalControl();
    }

    // Check if this is a jump transition or just falling
    bool isJumpTransition = context.IsJumping();

    if (isJumpTransition)
    {
      // This is a new jump (first or double) - apply jump force
      hasAppliedJump = true;
      lastJumpTime = Time.time;
      movement.Jump();

      // Play jump animation and sound
      animator.Play("Jump");
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);

      // Determine if this is first or double jump for logging
      string jumpType = context.GetCurrentJumps() == 1 ? "FIRST" : "DOUBLE";
      Debug.Log($"AIR STATE: Applied {jumpType} jump force - Jump #{context.GetCurrentJumps()}");

      // Clear the jump input to prevent stuck states
      context.ClearJumpingInput();
    }
    else
    {
      // Just falling (walked off edge, etc.) - don't use up a jump
      animator.Play("Fall");
      Debug.Log("AIR STATE: Entered falling (no jump used)");
    }

    UpdateAirMovementState();
  }

  public override void FixedUpdate()
  {
    airTimer += Time.fixedDeltaTime;

    // Handle air movement
    movement.MovementInAir(input.GetMovementInput());

    // Apply falling physics
    movement.Fall();

    // Update air movement state for animations
    UpdateAirMovementState();
  }

  private void UpdateAirMovementState()
  {
    float yVelocity = rb.linearVelocity.y;
    bool wasRising = isRising;
    bool wasFalling = isFalling;

    isRising = yVelocity > 0.5f;
    isFalling = yVelocity < -0.5f;

    // Update animations based on air movement
    if (isRising && !wasRising)
    {
      animator.Play("Jump");
    }
    else if (isFalling && !wasFalling)
    {
      animator.Play("Fall");
    }
  }

  public override IState CheckTransitions()
  {
    // Priority 0: Safety timeout - force grounding if in air too long
    if (airTimer > maxAirTime)
    {
      Debug.Log($"AirState: Safety timeout after {maxAirTime}s - forcing transition to Locomotion");
      movement.ForceStopExternalControl(); // Clear any stuck external control
      return context.GetState(PlayerState.Locomotion);
    }

    // Priority 1: Hurt state (highest priority) - preserve air state context
    if (context.IsHurt())
    {
      Debug.Log("AirState: Transitioning to Hurt (was in air)");
      return context.GetState(PlayerState.Hurt);
    }

    // Priority 2: Air dash
    if (context.IsDashing())
    {
      Debug.Log("AirState: Transitioning to Dash");
      return context.GetState(PlayerState.Dash);
    }

    // Priority 3: Double jump while in air - CHECK BEFORE LANDING
    if (!context.IsGrounded && context.IsJumping())
    {
      Debug.Log("AirState: Double jump detected - staying in Air state");
      // Trigger a new jump force application
      hasAppliedJump = true;
      lastJumpTime = Time.time;
      movement.Jump();

      // Play jump animation and sound for double jump
      animator.Play("Jump");
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);

      Debug.Log($"AirState: Applied double jump in transition - Jump #{context.GetCurrentJumps()}");
      context.ClearJumpingInput();

      // Stay in current air state (don't transition)
      return null;
    }

    // Priority 4: Landing - only transition if actually grounded
    if (context.IsGrounded)
    {
      Debug.Log("AirState: Transitioning to Land/Locomotion");
      // Check if we need a special landing state (high speed landing)
      if (Mathf.Abs(rb.linearVelocity.y) > 8f || movement.ShouldRollOnLanding())
      {
        return context.GetState(PlayerState.Land);
      }
      else
      {
        // Soft landing - go directly to locomotion
        return context.GetState(PlayerState.Locomotion);
      }
    }

    // Stay in air state
    return null;
  }

  public override void Exit()
  {
    Debug.Log("=== EXITED AIR STATE ===");
    Debug.Log($"Final Jumps Used: {context.GetCurrentJumps()}, Air Time: {airTimer:F2}s");

    // Clear any air-specific flags but let PlayerController handle jump reset on landing
    airTimer = 0f;
    hasAppliedJump = false;
  }
}
