using System.Collections;
using UnityEngine;

public class EnemyAttackState : BaseEnemyState
{
    private Animator animator;
    private GameObject attackPoint;
    private float attackTimer;
    private bool isAttacking;
    private Coroutine attackCoroutine;

    public EnemyAttackState(EnemyController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        attackPoint = context.EnemyStats.AttackPoint;
    }

    public override void Enter()
    {
        Debug.Log("Enemy entering Attack State");
        attackTimer = 0f;
        isAttacking = false;
        PerformAttack();
    }

    public override void FixedUpdate()
    {
        if (isAttacking)
        {
            attackTimer -= Time.fixedDeltaTime;
        }
    }

    private void PerformAttack()
    {
        if (isAttacking || attackTimer > 0f)
            return;

        isAttacking = true;
        attackTimer = context.EnemyStats.AttackCooldown;

        animator.Play("Attack");
        attackCoroutine = context.StartCoroutine(ResetAttackState());

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackPoint.transform.position,
            0.5f,
            LayerMask.GetMask("Player")
        );

        if (colliders.Length > 0)
        {
            foreach (Collider2D collider in colliders)
            {
                PlayerStats playerStats = collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(context.EnemyStats.AttackDamage);
                    Debug.Log(
                        $"Player hit by enemy attack! Damage: {context.EnemyStats.AttackDamage}"
                    );
                }
            }
        }
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public override IState CheckTransitions()
    {
        if (isAttacking)
        {
            return null;
        }

        if (context.EnemyStats.IsDead)
        {
            return context.GetState(EnemyState.Die);
        }

        if (context.EnemyStats.IsHurt)
        {
            return context.GetState(EnemyState.Hurt);
        }

        if (context.Player == null)
        {
            return context.GetState(EnemyState.Patrol);
        }

        if (!context.IsPlayerInAttackRange())
        {
            return context.GetState(EnemyState.Chase);
        }

        if (context.IsPlayerInAttackRange())
        {
            return context.GetState(EnemyState.Attack);
        }

        Debug.Log("No transition available, staying in Attack State");
        return null;
    }
}
