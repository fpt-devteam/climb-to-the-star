using UnityEngine;

public class WalkingState : BasePlayerState
{
    public WalkingState(PlayerController playerController, Animator animator)
        : base(playerController, animator) { }

    public override void OnEnter()
    {
        animator.Play("Walking");
        Debug.Log("WalkingState");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }

    public override void OnExit() { }
}
