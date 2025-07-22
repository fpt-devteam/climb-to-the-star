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
    // DEAD CELLS: Don't patrol if being knocked back, stunned, or hurt
    if (!context.CanPerformActions)
    {
      return;
    }

    // PROFESSIONAL: Don't patrol towards player if we can't attack (hurt/immune)
    if (context.IsPlayerInAttackRange() && !context.EnemyStats.CanAttack)
    {
      Debug.Log("Enemy near player but cannot attack due to hurt/immunity - continuing patrol");
    }
    else if (context.IsPlayerInAttackRange())
    {
      return; // Player in range and we can attack - let state machine handle transition
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
    // PROFESSIONAL: Priority 1 - Always check hurt state first
    if (context.EnemyStats.IsHurt)
    {
      Debug.Log("Enemy hurt during patrol - transitioning to hurt state");
      return context.GetState(EnemyState.Hurt);
    }

    // PROFESSIONAL: Priority 2 - Only attack if capable AND player in range
    if (context.IsPlayerInAttackRange() && context.EnemyStats.CanAttack)
    {
      Debug.Log("Enemy can attack player - transitioning to attack state");
      return context.GetState(EnemyState.Attack);
    }
    else if (context.IsPlayerInAttackRange() && !context.EnemyStats.CanAttack)
    {
      Debug.Log("Enemy near player but cannot attack (hurt/immune) - staying in patrol");
    }

    return null;
  }
}
