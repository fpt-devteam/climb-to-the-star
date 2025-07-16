using UnityEngine;

public class EnemyPatrolState : BaseEnemyState
{
    [SerializeField]
    private float patrolPointReachedThreshold = 0.5f;
    private Animator animator;

    public EnemyPatrolState(EnemyController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Run");
        Debug.Log("Enemy entering Patrol State");
    }

    public override void FixedUpdate()
    {
        if (context.IsPlayerInAttackRange())
        {
            return;
        }

        Vector2 patrolTarget = context.IsFacingRight
            ? context.RightPatrolPoint
            : context.LeftPatrolPoint;
        float distanceToTarget = Vector2.Distance(context.transform.position, patrolTarget);

        if (distanceToTarget <= patrolPointReachedThreshold)
        {
            context.ChangeDirection(!context.IsFacingRight);
        }

        context.MoveTowards(patrolTarget);
    }

    public override void Exit()
    {
        animator.StopPlayback();
    }

    public override IState CheckTransitions()
    {
        if (context.EnemyStats.IsHurt)
        {
            return context.GetState(EnemyState.Hurt);
        }

        if (context.IsPlayerInAttackRange())
        {
            return context.GetState(EnemyState.Attack);
        }

        return null;
    }
}
