using System.Collections;
using UnityEngine;

public class HurtState : BasePlayerState
{
    private Animator animator;
    private float currentHealth;
    private bool isAppliedAnimation = false;
    private bool isAlive = true;

    public HurtState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        currentHealth = context.PlayerStats.CurrentHealth;

        if (currentHealth <= 0)
        {
            isAlive = false;
            return;
        }

        animator.Play("Hurt");
        context.StartCoroutine(ExitToDefault());
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.4f);
        isAppliedAnimation = true;
    }

    public override IState CheckTransitions()
    {
        if (!isAlive)
        {
            return context.GetState(PlayerState.Die);
        }

        if (isAppliedAnimation)
        {
            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
