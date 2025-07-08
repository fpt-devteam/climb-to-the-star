using System.Collections;
using UnityEngine;

public class LandingState : BasePlayerState
{
    public LandingState(PlayerController playerController, Animator animator)
        : base(playerController, animator) { }

    public override void OnEnter()
    {
        animator.Play("Land");
        playerController.StartCoroutine(ExitToDefault());
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.4f);
        playerController.stateMachine.SetState(new IdleState(playerController, animator));
    }
}
