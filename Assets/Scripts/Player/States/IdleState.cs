using UnityEngine;

public class IdleState : BasePlayerState
{
    public IdleState(PlayerController playerController, Animator animator)
        : base(playerController, animator) { }

    public override void OnEnter()
    {
        animator.Play("Idle");
        Debug.Log("IdleState");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }

    public override void OnExit()
    {
        animator.Play("Idle", 0, 0);
    }
}
