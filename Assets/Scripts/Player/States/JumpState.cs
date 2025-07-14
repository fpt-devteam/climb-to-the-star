using System.Collections;
using UnityEngine;

public class JumpState : BasePlayerState
{
    private Animator animator;
    private bool hasApplyJump;
    private IPlayerMovement movement;
    private IPlayerInput input;

    public JumpState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        movement = context.PlayerMovement;
        input = context.PlayerInput;
    }

    public override void Enter()
    {
        animator.Play("Jump");
        hasApplyJump = false;
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);
    }

    public override void FixedUpdate()
    {
        if (!hasApplyJump)
        {
            hasApplyJump = true;
            movement.Jump();
        }
        movement.MovementInAir(input.GetMovementInput());
    }

    public override IState CheckTransitions()
    {
        if (context.IsFalling())
        {
            return context.GetState(PlayerState.Fall);
        }

        return null;
    }
}
