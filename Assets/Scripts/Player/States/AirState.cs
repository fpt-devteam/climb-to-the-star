using System.Collections;
using UnityEngine;

public class AirState : BasePlayerState
{
  private Animator animator;
  private IPlayerMovement movement;
  private IPlayerInput input;
  private Rigidbody2D rb;

  private bool hasAppliedJump = false;
  private bool isRising = false;
  private bool isFalling = false;
  private float airTimer = 0f;
  private float lastJumpTime = 0f;
  private float doubleJumpGraceTime = 0.08f;
  private float maxAirTime = 5f;

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

    if (movement.IsUnderExternalControl() && !context.IsJumping())
    {
      Debug.Log("AirState: Clearing stuck external control on air entry");
      movement.ForceStopExternalControl();
    }

    bool isJumpTransition = context.IsJumping();

    if (isJumpTransition)
    {
      hasAppliedJump = true;
      lastJumpTime = Time.time;
      movement.Jump();

      animator.Play("Jump");
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);

      string jumpType = context.GetCurrentJumps() == 1 ? "FIRST" : "DOUBLE";
      Debug.Log($"AIR STATE: Applied {jumpType} jump force - Jump #{context.GetCurrentJumps()}");

      context.ClearJumpingInput();
    }
    else
    {
      animator.Play("Fall");
      Debug.Log("AIR STATE: Entered falling (no jump used)");
    }

    UpdateAirMovementState();
  }

  public override void FixedUpdate()
  {
    airTimer += Time.fixedDeltaTime;

    movement.MovementInAir(input.GetMovementInput());

    movement.Fall();

    UpdateAirMovementState();
  }

  public override void Update()
  {
    // IMPROVED DOUBLE JUMP: Let PlayerController handle all jump logic including timing
    // This ensures consistent buffering and prevents early consumption of double jumps
    if (!context.IsGrounded && context.IsJumping())
    {
      Debug.Log("AirState: Valid double jump detected from PlayerController");

      // Apply the jump physics
      hasAppliedJump = true;
      lastJumpTime = Time.time;
      movement.Jump();

      // Visual and audio feedback
      animator.Play("Jump");
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);

      Debug.Log($"AirState: Applied double jump - Jump #{context.GetCurrentJumps()}");
      context.ClearJumpingInput();
    }
  }

  private void UpdateAirMovementState()
  {
    float yVelocity = rb.linearVelocity.y;
    bool wasRising = isRising;
    bool wasFalling = isFalling;

    isRising = yVelocity > 0.5f;
    isFalling = yVelocity < -0.5f;

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
    if (movement.IsUnderExternalControl() && context.IsGrounded && airTimer > 0.5f)
    {
      Debug.Log("AirState: Force clearing stuck external control - player should have landed");
      movement.ForceStopExternalControl();
    }

    if (airTimer > maxAirTime)
    {
      Debug.Log($"AirState: Safety timeout after {maxAirTime}s - forcing transition to Locomotion");
      movement.ForceStopExternalControl(); return context.GetState(PlayerState.Locomotion);
    }

    if (context.IsHurt())
    {
      Debug.Log("AirState: Transitioning to Hurt (was in air)");
      return context.GetState(PlayerState.Hurt);
    }

    if (context.IsDashing() && !movement.IsUnderExternalControl())
    {
      Debug.Log("AirState: Transitioning to Dash");
      return context.GetState(PlayerState.Dash);
    }

    if (context.IsGrounded)
    {
      Debug.Log("AirState: Transitioning to Locomotion (landing)");

      if (movement.IsUnderExternalControl())
      {
        Debug.Log("AirState: Clearing external control on landing");
        movement.ForceStopExternalControl();
      }

      return context.GetState(PlayerState.Locomotion);
    }

    return null;
  }

  public override void Exit()
  {
    Debug.Log("=== EXITED AIR STATE ===");
    Debug.Log($"Final Jumps Used: {context.GetCurrentJumps()}, Air Time: {airTimer:F2}s");

    airTimer = 0f;
    hasAppliedJump = false;
  }
}
