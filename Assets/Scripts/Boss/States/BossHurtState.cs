using System.Collections;
using UnityEngine;

public class BossHurtState : BaseBossState
{
    private float hurtDuration;
    private bool isAppliedAnimation;
    private Animator animator;

    public BossHurtState(BossController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        hurtDuration = context.BossStats.ImmuneDuration;
    }

    public override void Enter()
    {
        Debug.Log("Boss entering Hurt State");
        AudioManager.Instance.PlaySFX(AudioSFXEnum.BossHurt);

        isAppliedAnimation = false;
        animator.Play("Hurt");
        context.StartCoroutine(ExitHurtState());
    }

    private IEnumerator ExitHurtState()
    {
        yield return new WaitForSeconds(hurtDuration);
        isAppliedAnimation = true;
    }

    public override void Exit()
    {
        animator.StopPlayback();
    }

    public override IState CheckTransitions()
    {
        if (context.BossStats.IsDead)
        {
            return context.GetState(BossState.Death);
        }

        if (isAppliedAnimation)
        {
            return context.GetState(BossState.Search);
        }

        Debug.Log("Boss is still hurt, staying in Hurt State");
        return null;
    }
}
