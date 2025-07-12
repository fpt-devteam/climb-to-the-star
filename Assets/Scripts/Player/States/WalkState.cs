using UnityEngine;

public class WalkState : BasePlayerState
{
    private readonly Animator animator;

    public WalkState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Walk");
    }

    public override void Update()
    {
        var direction = context.PlayerInput.GetMovementInput();
        context.PlayerMovement.Move(direction);
    }

    public override void FixedUpdate() { }

    public override void Exit() { }

    public override IState CheckTransitions()
    {
        if (context.IsGrounded() && context.IsIdling())
            return context.GetState(PlayerState.Idle);
        return null;
    }
}
