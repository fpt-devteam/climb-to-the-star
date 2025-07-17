using System.Collections;
using UnityEngine;

public class LocomotionState : BasePlayerState
{
  private readonly Animator animator;
  private IPlayerMovement movement;
  private IPlayerInput input;

  public LocomotionState(PlayerController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    movement = context.PlayerMovement;
    input = context.PlayerInput;
  }

  public override void Enter()
  {
    // Start with appropriate animation based on current state
    UpdateAnimation();
  }

  public override void FixedUpdate()
  {
    // Only handle movement if not under external control (like dash)
    if (!movement.IsUnderExternalControl())
    {
      HandleMovement();
    }
    UpdateAnimation();
  }

  private void HandleMovement()
  {
    if (context.IsWalking())
    {
      OnMovement();
    }
    // Physics system handles deceleration automatically when no input
  }

  private void OnMovement()
  {
    movement.Move(input.GetMovementInput());
  }

  private void UpdateAnimation()
  {
    // Use actual velocity for smoother animation transitions
    if (movement.IsMovingHorizontally())
    {
      animator.Play("Walk");
    }
    else
    {
      animator.Play("Idle");
    }
  }

  public override IState CheckTransitions()
  {
    // DEAD CELLS: Air actions first (highest priority) - work anywhere
    if (context.IsDashing())
    {
      return context.GetState(PlayerState.Dash);
    }

    if (context.IsJumping())
    {
      return context.GetState(PlayerState.Air); // Use unified AirState
    }

    // Ground actions
    if (context.IsCharging())
    {
      return context.GetState(PlayerState.Charge);
    }

    if (context.IsShielding())
    {
      return context.GetState(PlayerState.Shield);
    }

    if (context.CanAttack())
    {
      return context.GetState(PlayerState.Attack1);
    }

    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    // Check for falling last - transition to AirState
    if (context.IsFalling())
    {
      return context.GetState(PlayerState.Air); // Use unified AirState
    }

    return null;
  }
}
