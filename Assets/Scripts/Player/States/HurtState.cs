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
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerHurt);
    }

    public override IState CheckTransitions()
    {
        if (!isAlive)
        {
            return context.GetState(PlayerState.Die);
        }

        if (!context.IsHurt())
        {
            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
