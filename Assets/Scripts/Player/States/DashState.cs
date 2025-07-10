using System.Collections;
using UnityEngine;

public class DashState : BasePlayerState
{
    private Animator animator;

    public DashState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Dash");
        playerController.HandleDash();
        playerController.StartCoroutine(ExitToDefault());
        Debug.Log("Dash");
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(1f);
        playerController.stateMachine.SetState(new IdleState(playerController));
    }
}
