using UnityEngine;

public class FallState : BasePlayerState
{
    public FallState(PlayerController playerController, Animator animator)
        : base(playerController, animator) { }

    public override void OnEnter()
    {
        animator.Play("Fall");
        Debug.Log("FallState");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }

    public override void OnExit() { }
}
