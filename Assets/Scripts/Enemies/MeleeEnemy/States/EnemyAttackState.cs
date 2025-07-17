using System.Collections;
using UnityEngine;

public class EnemyAttackState : BaseEnemyState
{
  [Header("Attack Settings")]
  [SerializeField]
  private float attackCooldown = 1.5f;

  [SerializeField]
  private float attackDelay = 0.3f;

  private Animator animator;
  private GameObject attackPoint;
  private bool isAttacking;
  private float lastAttackTime;

  public EnemyAttackState(EnemyController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    attackPoint = context.EnemyStats.AttackPoint;
  }

  public override void Enter()
  {
    Debug.Log("Enemy entering Attack State");

    // PROFESSIONAL: Cannot attack while hurt/immune (blinking)
    if (!context.EnemyStats.CanAttack)
    {
      Debug.Log("Enemy cannot attack while hurt/immune - returning to patrol");
      return;
    }

    // DEAD CELLS: Cannot attack while knocked back or stunned
    if (!context.CanPerformActions)
    {
      Debug.Log("Enemy cannot attack while knocked back/stunned - returning to patrol");
      return;
    }

    if (Time.time - lastAttackTime < attackCooldown)
    {
      Debug.Log("Attack on cooldown, returning to patrol");
      return;
    }

    isAttacking = true;
    lastAttackTime = Time.time;

    animator.Play("Attack");
    AudioManager.Instance.PlaySFX(AudioSFXEnum.EnemyAttack);

    // Delay the attack to match animation timing
    context.StartCoroutine(PerformDelayedAttack());

    context.StartCoroutine(ResetAttackState());
  }

  private IEnumerator PerformDelayedAttack()
  {
    yield return new WaitForSeconds(attackDelay);

    // PROFESSIONAL: Double-check attack capability before executing
    if (context.EnemyStats.CanAttack)
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
    Collider2D collider = Physics2D.OverlapCircle(
        attackPoint.transform.position,
        context.EnemyStats.AttackRange,
        LayerMask.GetMask("Player")
    );

    if (collider != null)
    {
      PlayerStats playerStats = collider.GetComponent<PlayerStats>();
      if (playerStats != null && !playerStats.IsInvincible) // DEAD CELLS: Check all invincibility types
      {
        playerStats.TakeDamage(context.EnemyStats.AttackDamage);

        // DEAD CELLS: Screen shake when enemy hits player
        CameraShake.PlayerHurt();

        Debug.Log($"Player hit by enemy attack! Damage: {context.EnemyStats.AttackDamage}");
      }
    }
  }

  private IEnumerator ResetAttackState()
  {
    yield return new WaitForSeconds(1f);
    isAttacking = false;
  }

  public override IState CheckTransitions()
  {
    // PROFESSIONAL: Priority 1 - Always transition to hurt if hurt/immune
    if (context.EnemyStats.IsHurt)
    {
      Debug.Log("Enemy hurt during attack - transitioning to hurt state");
      return context.GetState(EnemyState.Hurt);
    }

    // PROFESSIONAL: Priority 2 - Cancel attack if can't attack anymore
    if (isAttacking && !context.EnemyStats.CanAttack)
    {
      Debug.Log("Enemy lost attack capability - returning to patrol");
      isAttacking = false;
      return context.GetState(EnemyState.Patrol);
    }

    // Priority 3 - Continue attacking if still attacking
    if (isAttacking)
    {
      return null;
    }

    // Priority 4 - Return to patrol when attack complete
    return context.GetState(EnemyState.Patrol);
  }

  private void OnDrawGizmosSelected()
  {
    if (attackPoint != null)
    {
      // Visual feedback for attack state
      Gizmos.color = context.EnemyStats.CanAttack ? Color.red : Color.gray;
      Gizmos.DrawWireSphere(attackPoint.transform.position, context.EnemyStats.AttackRange);
    }
  }
}
