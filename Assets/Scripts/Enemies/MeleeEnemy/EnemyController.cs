using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Component References")]
    private EnemyStats enemyStats;
    private Rigidbody2D rb;

    [SerializeField]
    private GameObject startPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;

    private StateMachine stateMachine;
    private Dictionary<EnemyState, IState> states;

    private bool isFacingRight = true;
    private PlayerStats player;

    public EnemyStats EnemyStats => enemyStats;
    public bool IsFacingRight => isFacingRight;
    public PlayerStats Player => player;
    public Vector2 LeftPatrolPoint => leftPatrolPoint;
    public Vector2 RightPatrolPoint => rightPatrolPoint;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateMachine();

        leftPatrolPoint =
            (Vector2)startPosition.transform.position + Vector2.left * enemyStats.PatrolDistance;
        rightPatrolPoint =
            (Vector2)startPosition.transform.position + Vector2.right * enemyStats.PatrolDistance;
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
    }

    private void InitializeStateMachine()
    {
        states = new Dictionary<EnemyState, IState>
        {
            { EnemyState.Patrol, new EnemyPatrolState(this) },
            { EnemyState.Chase, new EnemyChaseState(this) },
            { EnemyState.Attack, new EnemyAttackState(this) },
            { EnemyState.Hurt, new EnemyHurtState(this) },
            { EnemyState.Die, new EnemyDieState(this) },
        };

        stateMachine = new StateMachine();
        stateMachine.Initialize(GetState(EnemyState.Patrol));
    }

    private void Update()
    {
        HandleFacingDirection();
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void HandleFacingDirection()
    {
        if (player != null)
        {
            float directionToPlayer = player.transform.position.x - transform.position.x;

            if (directionToPlayer > 0f && !isFacingRight)
            {
                transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = true;
            }
            else if (directionToPlayer < 0f && isFacingRight)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isFacingRight = false;
            }
        }
    }

    public void SetPlayer(PlayerStats player)
    {
        this.player = player;
    }

    public bool IsPlayerInAttackRange()
    {
        if (player == null)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return IsPlayerInDetectionRange() && distanceToPlayer <= enemyStats.AttackRange;
    }

    public bool IsPlayerInDetectionRange()
    {
        if (player == null)
            return false;

        var xPosition = transform.position.x;
        var yPosition = transform.position.y;
        var leftBound = Mathf.Min(leftPatrolPoint.x, rightPatrolPoint.x);
        var rightBound = Mathf.Max(leftPatrolPoint.x, rightPatrolPoint.x);

        return leftBound <= xPosition && xPosition <= rightBound && yPosition == leftPatrolPoint.y;
    }

    public void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            enemyStats.MoveSpeed * Time.fixedDeltaTime
        );
    }

    public void SetDirection(bool facingRight)
    {
        isFacingRight = facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }

    public IState GetState(EnemyState state) =>
        states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftPatrolPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPatrolPoint, 0.3f);

        Gizmos.color = Color.red;
        Vector3 patrolAreaCenter = (Vector3)(leftPatrolPoint + rightPatrolPoint) * 0.5f;
        Vector3 patrolAreaSize = new Vector3(EnemyStats.PatrolDistance * 2, 2f, 0f);
        Gizmos.DrawWireCube(patrolAreaCenter, patrolAreaSize);
    }
}
