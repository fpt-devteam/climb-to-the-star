using System.Collections;
using UnityEngine;

public class EnemyAttackState : BaseEnemyState
{
    [Header("Attack Settings")]
    [SerializeField]
    private float attackCooldown = 1.5f;

    [SerializeField]
    private float attackRange = 0.5f;

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

        if (Time.time - lastAttackTime < attackCooldown)
        {
            Debug.Log("Attack on cooldown, returning to patrol");
            return;
        }

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.Play("Attack");
        AudioManager.Instance.PlaySFX(AudioSFXEnum.EnemyAttack);

        PerformAttack();

        context.StartCoroutine(ResetAttackState());
    }

    private void PerformAttack()
    {
        Collider2D collider = Physics2D.OverlapCircle(
            attackPoint.transform.position,
            attackRange,
            LayerMask.GetMask("Player")
        );

        if (collider != null)
        {
            PlayerStats playerStats = collider.GetComponent<PlayerStats>();
            if (playerStats != null && !playerStats.IsImmune)
            {
                playerStats.TakeDamage(context.EnemyStats.AttackDamage);
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
        if (context.EnemyStats.IsHurt)
        {
            return context.GetState(EnemyState.Hurt);
        }

        if (isAttacking)
        {
            return null;
        }

        return context.GetState(EnemyState.Patrol);
    }
}
