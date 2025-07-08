using UnityEngine;

public class JumpState : BasePlayerState
{
    private bool hasAppliedJumpForce = false;

    public JumpState(PlayerController playerController, Animator animator)
        : base(playerController, animator) { }

    public override void OnEnter()
    {
        animator.Play("Jump");
        hasAppliedJumpForce = false;
        Debug.Log("JumpState");
    }

    public override void FixedUpdate()
    {
        if (!hasAppliedJumpForce)
        {
            playerController.HandleJump();
            hasAppliedJumpForce = true;
        }
        playerController.HandleMovement();
    }

    public override void OnExit()
    {
        hasAppliedJumpForce = false;
    }
}
