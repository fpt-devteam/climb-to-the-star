using UnityEngine;

public class EnemyPatrolState : BaseEnemyState
{
    [SerializeField]
    private float patrolPointReachedThreshold = 0.5f;

    [SerializeField]
    private float patrolDistance = 3f;

    private bool isMovingRight;

    private Animator animator;

    private Vector2 startPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugGizmos = true;

    public EnemyPatrolState(EnemyController enemyController)
        : base(enemyController)
    {
        animator = enemyController.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        animator.Play("Run");
        startPosition = enemyController.transform.position;
        leftPatrolPoint = startPosition + Vector2.left * patrolDistance;
        rightPatrolPoint = startPosition + Vector2.right * patrolDistance;
        isMovingRight = true;

        Debug.Log("Start Position: " + startPosition);
        Debug.Log("Left Patrol Point: " + leftPatrolPoint);
        Debug.Log("Right Patrol Point: " + rightPatrolPoint);
    }

    public override void FixedUpdate()
    {
        CheckPatrolPointReached();
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
            isMovingRight = !isMovingRight;
        }
    }

    private Vector2 GetPatrolTarget() => isMovingRight ? rightPatrolPoint : leftPatrolPoint;

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(startPosition, new Vector3(patrolDistance * 2, 1, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftPatrolPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPatrolPoint, 0.3f);
    }
}
