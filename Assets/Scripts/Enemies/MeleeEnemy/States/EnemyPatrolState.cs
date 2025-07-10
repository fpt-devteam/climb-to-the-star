using UnityEngine;

public class EnemyPatrolState : BaseEnemyState
{
    [SerializeField]
    private float patrolPointReachedThreshold = 0.5f;

    public EnemyPatrolState(EnemyController enemyController)
        : base(enemyController) { }

    public override void OnEnter()
    {
        animator.Play("Run");
    }

    public override void FixedUpdate()
    {
        CheckPatrolPointReached();
        MoveTowardsPatrolTarget();
    }

    public override void OnExit() { }

    private void MoveTowardsPatrolTarget()
    {
        enemyController.MoveTowards(GetPatrolTarget());
    }

    private void CheckPatrolPointReached()
    {
        float distanceToTarget = Vector2.Distance(
            enemyController.transform.position,
            GetPatrolTarget()
        );

        if (distanceToTarget <= patrolPointReachedThreshold)
        {
            enemyController.isMovingRight = !enemyController.isMovingRight;
        }
    }

    private Vector2 GetPatrolTarget() =>
        enemyController.isMovingRight
            ? enemyController.rightPatrolPoint
            : enemyController.leftPatrolPoint;
}
