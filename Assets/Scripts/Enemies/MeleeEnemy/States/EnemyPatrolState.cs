using UnityEngine;

public class EnemyPatrolState : BaseEnemyState
{
    [SerializeField]
    private float patrolPointReachedThreshold = 0.5f;
    private Animator animator;

    private bool isMovingRight;

    public EnemyPatrolState(EnemyController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Run");
        isMovingRight = context.IsFacingRight;
    }

    public override void FixedUpdate()
    {
        CheckPatrolPointReached();
        HandleFacingDirection();
        context.MoveTowards(GetPatrolTarget());
        DetectPlayer();
    }

    private void CheckPatrolPointReached()
    {
        float distanceToTarget = Vector2.Distance(context.transform.position, GetPatrolTarget());

        if (distanceToTarget <= patrolPointReachedThreshold)
        {
            isMovingRight = !isMovingRight;
        }
    }

    private void HandleFacingDirection()
    {
        if (isMovingRight && !context.IsFacingRight)
        {
            context.SetDirection(true);
        }
        else if (!isMovingRight && context.IsFacingRight)
        {
            context.SetDirection(false);
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            context.transform.position,
            context.EnemyStats.PlayerDetectionRange,
            LayerMask.GetMask("Player")
        );

        if (playerCollider != null)
        {
            PlayerStats detectedPlayer = playerCollider.GetComponent<PlayerStats>();

            if (detectedPlayer != null)
            {
                float playerX = detectedPlayer.transform.position.x;
                float leftBound = Mathf.Min(context.LeftPatrolPoint.x, context.RightPatrolPoint.x);
                float rightBound = Mathf.Max(context.LeftPatrolPoint.x, context.RightPatrolPoint.x);

                if (playerX >= leftBound && playerX <= rightBound)
                {
                    context.SetPlayer(detectedPlayer);
                    Debug.Log("Player detected in patrol area");
                }
                else
                {
                    context.SetPlayer(null);
                }
            }
        }
        else
        {
            context.SetPlayer(null);
        }
    }

    private Vector2 GetPatrolTarget() =>
        isMovingRight ? context.RightPatrolPoint : context.LeftPatrolPoint;

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

        if (context.Player != null)
        {
            if (context.IsPlayerInAttackRange())
            {
                return context.GetState(EnemyState.Attack);
            }
            else if (context.IsPlayerInDetectionRange())
            {
                return context.GetState(EnemyState.Chase);
            }
        }

        return null;
    }
}
