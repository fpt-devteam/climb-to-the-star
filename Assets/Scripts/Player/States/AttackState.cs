using System.Collections;
using UnityEngine;

public class AttackState : BasePlayerState
{
    private Animator animator;

    public AttackState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        playerController.HandleSkill();
        playerController.StartCoroutine(ExitToDefault());
        Debug.Log("Attacking");
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.4f);
        playerController.stateMachine.SetState(new IdleState(playerController));
    }
}
