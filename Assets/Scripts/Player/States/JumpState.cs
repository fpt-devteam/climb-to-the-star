using UnityEngine;

public class JumpState : BasePlayerState
{
    private Animator animator;
    private bool hasAppliedJumpForce = false;

    public JumpState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Jump");
        hasAppliedJumpForce = false;
        Debug.Log("Jumping");
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
}
