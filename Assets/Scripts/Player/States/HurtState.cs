using System.Collections;
using UnityEngine;

public class HurtState : BasePlayerState
{
    private Animator animator;
    private float currentHealth;

    public HurtState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        currentHealth = playerController.GetComponent<Player>().CurrentHealth();
        if (currentHealth <= 0)
        {
            playerController.stateMachine.SetState(new DieState(playerController));
            return;
        }

        animator.Play("Hurt");
        playerController.StartCoroutine(ExitToDefault());
        Debug.Log("Hurt");
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.4f);
        playerController.stateMachine.SetState(new IdleState(playerController));
    }
}
