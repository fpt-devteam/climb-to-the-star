using System.Collections;
using UnityEngine;

public class FallState : BasePlayerState
{
    private Animator animator;
    private IPlayerMovement movement;
    private IPlayerInput input;

    public FallState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        movement = context.PlayerMovement;
        input = context.PlayerInput;
    }

    public override void Enter()
    {
        animator.Play("Fall");
    }

    public override void FixedUpdate()
    {
        movement.Fall();
        movement.MovementInAir(input.GetMovementInput());
    }

    public override IState CheckTransitions()
    {
        if (context.IsGrounded)
        {
            return context.GetState(PlayerState.Land);
        }

        return null;
    }
}
