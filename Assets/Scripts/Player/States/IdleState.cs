using UnityEngine;

public class IdleState : BasePlayerState
{
    private readonly Animator animator;

    public IdleState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Idle");
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    public override void Exit() { }

    public override IState CheckTransitions()
    {
        if (context.IsGrounded() && context.IsWalking())
            return context.GetState(PlayerState.Walk);
        return null;
    }
}
