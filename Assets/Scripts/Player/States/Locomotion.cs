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
        OnIdle();
    }

    public override void FixedUpdate()
    {
        if (context.IsWalking())
        {
            OnWalk();
            OnMovement();
            return;
        }

        if (context.IsIdling())
        {
            OnIdle();
        }
    }

    private void OnWalk()
    {
        animator.Play("Walk");
    }

    private void OnMovement()
    {
        movement.Move(input.GetMovementInput());
    }

    private void OnIdle()
    {
        animator.Play("Idle");
    }

    public override IState CheckTransitions()
    {
        if (context.IsCharging())
        {
            return context.GetState(PlayerState.Charge);
        }

        if (context.IsShielding())
        {
            return context.GetState(PlayerState.Shield);
        }

        if (context.IsDashing())
        {
            return context.GetState(PlayerState.Dash);
        }

        if (context.CanAttack())
        {
            return context.GetState(PlayerState.Attack1);
        }

        if (context.IsHurt())
        {
            return context.GetState(PlayerState.Hurt);
        }

        if (context.IsJumping())
        {
            return context.GetState(PlayerState.Jump);
        }

        if (context.IsFalling())
        {
            return context.GetState(PlayerState.Fall);
        }

        return null;
    }
}
