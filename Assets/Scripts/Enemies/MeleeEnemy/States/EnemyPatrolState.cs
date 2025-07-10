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
        Debug.Log("Enemy: Entered Patrol State");
    }

    public override void FixedUpdate()
    {
        CheckPatrolPointReached();
        MoveTowardsPatrolTarget();
    }

    public override void OnExit()
    {
        Debug.Log("Enemy: Exited Patrol State");
    }

    private void MoveTowardsPatrolTarget()
    {
        enemyController.MoveTowards(GetPatrolTarget());
    }

    private void CheckPatrolPointReached()
    {
        Debug.Log($"Checking Patrol Point Reached: {enemyController.isMovingRight}");
        Debug.Log(
            $"Right Patrol Point: {enemyController.rightPatrolPoint}, Left Patrol Point: {enemyController.leftPatrolPoint}"
        );

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
