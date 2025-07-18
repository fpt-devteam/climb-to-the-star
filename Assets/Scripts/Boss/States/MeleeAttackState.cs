using System.Collections;
using UnityEngine;

public class MeleeAttackState : BaseBossState
{
  private float attackCooldown = 1.5f;
  private float attackDelay = 0.3f;
  private float attackRange;
  private Coroutine animationCoroutine;

  private GameObject attackPoint;
  private Animator animator;
  private bool isAttacking;
  private float lastAttackTime;

  public MeleeAttackState(BossController context)
      : base(context)
  {
    attackPoint = context.BossStats.MeleeAttackPoint;
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    if (Time.time - lastAttackTime < attackCooldown)
    {
      return;
    }

    isAttacking = true;
    lastAttackTime = Time.time;

    attackCooldown = context.BossStats.AttackCooldown;
    attackRange = context.BossStats.MeleeAttackRange;

    Debug.Log("Boss entering Melee Attack State");
    animator.Play("MeleeAttack");
    context.StartCoroutine(PerformDelayedAttack());
    context.StartCoroutine(ResetAttackState());
  }

  private IEnumerator ResetAttackState()
  {
    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

  private IEnumerator PerformDelayedAttack()
  {
    yield return new WaitForSeconds(attackDelay);

    if (context.BossStats.CanAttack)
    {
      PerformAttack();
    }
    else
    {
      Debug.Log("Enemy became hurt/immune during attack delay - attack cancelled");
      isAttacking = false;
    }
  }
  private void PerformAttack()
  {
    AudioManager.Instance.PlaySFX(AudioSFXEnum.BossAttack);
  }

  public override void Exit()
  {
    if (animationCoroutine != null)
    {
      context.StopCoroutine(animationCoroutine);
      animationCoroutine = null;
    }
  }

  public override IState CheckTransitions()
  {
    if (context.BossStats.IsHurt)
    {
      return context.GetState(BossState.Hurt);
    }

    if (isAttacking && !context.BossStats.CanAttack)
    {
      isAttacking = false;
      return context.GetState(BossState.Idle);
    }

    if (isAttacking)
    {
      return null;
    }

    return context.GetState(BossState.Idle);
  }
}
