using System.Collections;
using UnityEngine;

public class LocomotionState : BasePlayerState
{
    private readonly Animator animator;
    private IPlayerMovement movement;
    private IPlayerInput input;

    private bool hasApplyJump;
    private bool hasApplyLand;

    public LocomotionState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        movement = context.PlayerMovement;
        input = context.PlayerInput;
    }

    public override void Enter()
    {
        hasApplyLand = true;
        hasApplyJump = false;

        OnIdle();
    }

    public override void FixedUpdate()
    {
        if (context.IsJumping())
        {
            if (!hasApplyJump)
            {
                hasApplyJump = true;
                OnJump();
            }

            OnMovement();
            return;
        }

        hasApplyJump = false;

        if (context.IsFalling())
        {
            hasApplyLand = false;
            OnFall();
            OnMovement();
            return;
        }

        if (context.IsWalking())
        {
            OnWalk();
            OnMovement();
            return;
        }

        if (context.IsIdling())
        {
            if (hasApplyLand)
            {
                OnIdle();
            }
            else
            {
                OnLand();
                context.StartCoroutine(WaitForTime(0.5f));
            }
        }
    }

    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        hasApplyLand = true;
    }

    private void OnWalk()
    {
        animator.Play("Walk");
    }

    private void OnJump()
    {
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerJump);
        animator.Play("Jump");
        movement.Jump();
    }

    private void OnFall()
    {
        animator.Play("Fall");
        movement.Fall();
    }

    private void OnMovement()
    {
        movement.Move(input.GetMovementInput());
    }

    private void OnIdle()
    {
        animator.Play("Idle");
    }

    private void OnLand()
    {
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerLand);
        animator.Play("Land");
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

        return null;
    }
}
