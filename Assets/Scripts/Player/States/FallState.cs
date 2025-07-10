using UnityEngine;

public class FallState : BasePlayerState
{
    private Animator animator;

    public FallState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Fall");
        Debug.Log("Falling");
    }

    public override void FixedUpdate()
    {
        playerController.HandleMovement();
    }
}
