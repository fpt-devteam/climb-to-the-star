using UnityEngine;

public class IdleState : BasePlayerState
{
    private Animator animator;

    public IdleState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Idle");
        Debug.Log("Idle");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }
}
