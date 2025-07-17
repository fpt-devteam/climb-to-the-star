using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Component References")]
    private EnemyStats enemyStats;
    private Rigidbody2D rb;

  // DEAD CELLS: Knockback system integration
  private EnemyKnockback knockbackComponent;

    [SerializeField]
    private GameObject startPosition;
    private Vector2 leftPatrolPoint;
    private Vector2 rightPatrolPoint;
    private StateMachine stateMachine;
    private Dictionary<EnemyState, IState> states;
    private PlayerStats player;
    private bool isFacingRight = true;

    public bool IsFacingRight => isFacingRight;
    public EnemyStats EnemyStats => enemyStats;
    public PlayerStats Player => player;
    public Vector2 LeftPatrolPoint => leftPatrolPoint;
    public Vector2 RightPatrolPoint => rightPatrolPoint;

  // DEAD CELLS: Check if enemy can perform actions (not knocked back/stunned)
  public bool CanPerformActions => knockbackComponent == null || knockbackComponent.CanPerformActions();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

    // DEAD CELLS: Initialize knockback component
    knockbackComponent = GetComponent<EnemyKnockback>();
    if (knockbackComponent == null)
    {
      knockbackComponent = gameObject.AddComponent<EnemyKnockback>();
      Debug.Log($"EnemyController: Added EnemyKnockback component to {gameObject.name}");
    }

        states = new Dictionary<EnemyState, IState>
        {
            { EnemyState.Patrol, new EnemyPatrolState(this) },
            { EnemyState.Attack, new EnemyAttackState(this) },
            { EnemyState.Hurt, new EnemyHurtState(this) },
            { EnemyState.Die, new EnemyDieState(this) },
        };

        stateMachine = new StateMachine();
        stateMachine.Initialize(GetState(EnemyState.Patrol));

        leftPatrolPoint =
            (Vector2)startPosition.transform.position + Vector2.left * enemyStats.PatrolDistance;
        rightPatrolPoint =
            (Vector2)startPosition.transform.position + Vector2.right * enemyStats.PatrolDistance;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        if (IsPlayerInAttackRange())
        {
            Vector2 direction = player.transform.position - transform.position;
            ChangeDirection(direction.x > 0);
        }
    }

    public void SetPlayer(PlayerStats player) => this.player = player;

    public bool IsPlayerInAttackRange()
    {
        if (player == null)
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
    bool inRange = distanceToPlayer <= enemyStats.AttackRange;

    // PROFESSIONAL: Add debug info for attack capability
    if (inRange && !enemyStats.CanAttack)
    {
      Debug.Log($"Player in range but enemy {gameObject.name} cannot attack (hurt/immune)");
    }

    return inRange;
  }

  public void MoveTowards(Vector2 targetPosition)
  {
    // DEAD CELLS: Don't move if being knocked back or stunned
    if (knockbackComponent != null && !knockbackComponent.CanMove)
    {
      return;
    }

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            enemyStats.MoveSpeed * Time.fixedDeltaTime
        );
  }

    public void ChangeDirection(bool facingRight)
    {
        isFacingRight = facingRight;

        transform.localScale = new Vector3(
            isFacingRight ? 1f : -1f,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    public IState GetState(EnemyState state) =>
        states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

    private void OnDrawGizmosSelected()
    {
    // Patrol points
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftPatrolPoint, 0.3f);
        Gizmos.DrawWireSphere(rightPatrolPoint, 0.3f);

    // Patrol area
    Gizmos.color = Color.green;
        Vector3 patrolAreaCenter = (Vector3)(leftPatrolPoint + rightPatrolPoint) * 0.5f;
        Vector3 patrolAreaSize = new Vector3(EnemyStats.PatrolDistance * 2, 2f, 0f);
        Gizmos.DrawWireCube(patrolAreaCenter, patrolAreaSize);

    // PROFESSIONAL: Attack range with capability feedback
    if (enemyStats != null)
    {
      // Red = can attack, Gray = hurt/immune (can't attack), Yellow = dead
      Color attackColor = Color.red;
      if (enemyStats.IsDead)
      {
        attackColor = Color.yellow;
      }
      else if (!enemyStats.CanAttack)
      {
        attackColor = Color.gray;
      }

      Gizmos.color = attackColor;
      Gizmos.DrawWireSphere(transform.position, enemyStats.AttackRange);

#if UNITY_EDITOR
      // Draw attack capability status text (in Scene view only)
      UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
          $"Attack: {(enemyStats.CanAttack ? "READY" : "BLOCKED")}");
#endif
    }
    }
}
