using UnityEngine;

public class EnemyChaseState : BaseEnemyState
{
    private Animator animator;

    public EnemyChaseState(EnemyController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Run");
        Debug.Log("Enemy entering Chase State");
    }

    public override void FixedUpdate()
    {
        if (context.Player != null)
        {
            context.MoveTowards(context.Player.transform.position);
        }
    }

    public override IState CheckTransitions()
    {
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

        if (context.IsPlayerInAttackRange())
        {
            return context.GetState(EnemyState.Attack);
        }

        if (!context.IsPlayerInDetectionRange())
        {
            return context.GetState(EnemyState.Patrol);
        }

        return null;
    }
}
