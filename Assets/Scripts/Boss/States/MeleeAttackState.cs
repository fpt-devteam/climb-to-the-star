using System.Collections;
using UnityEngine;

public class MeleeAttackState : BaseBossState
{
  private float attackDuration;
  private float attackRange;
  private bool hasAppliedAnimation;
  private Coroutine animationCoroutine;
  private bool hasAppliedDamage = false;

  private GameObject attackPoint;
  private Animator animator;

  public MeleeAttackState(BossController context)
      : base(context)
  {
    attackPoint = context.BossStats.MeleeAttackPoint;
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    hasAppliedAnimation = false;
    hasAppliedDamage = false;
    attackDuration = context.BossStats.AttackCooldown;
    attackRange = context.BossStats.MeleeAttackRange;

    // AudioManager.Instance.PlaySFX(AudioSFXEnum.BossAttack);
    Debug.Log("Boss entering Melee Attack State");
    animator.Play("MeleeAttack");
    animationCoroutine = context.StartCoroutine(WaitForAnimationCompletion());
    PerformDamage();
  }

  public override void Exit()
  {
    if (animationCoroutine != null)
    {
      context.StopCoroutine(animationCoroutine);
      animationCoroutine = null;
    }
  }

  protected virtual void PerformDamage()
  {
    float baseDamage = context.BossStats.MeleeAttackDamage;
    float finalDamage = baseDamage;

    Collider2D collider = Physics2D.OverlapCircle(attackPoint.transform.position, context.BossStats.MeleeAttackRange, LayerMask.GetMask("Player"));

    if (collider != null)
    {
      PlayerStats playerStats = collider.GetComponent<PlayerStats>();
      if (playerStats != null)
      {
        playerStats.TakeDamage(finalDamage);
      }
    }
  }

  protected IEnumerator WaitForAnimationCompletion()
  {
    yield return new WaitForSeconds(attackDuration);
    hasAppliedAnimation = true;
  }

  public override IState CheckTransitions()
  {
    if (context.BossStats.IsHurt)
    {
      return context.GetState(BossState.Hurt);
    }

    if (hasAppliedAnimation)
    {
      return context.GetState(BossState.Idle);
    }

    return null;
  }
}
