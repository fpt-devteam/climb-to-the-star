using UnityEngine;

public class WalkingState : BasePlayerState
{
    private Animator animator;

    public WalkingState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Walking");
        Debug.Log("Walking");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }
}
