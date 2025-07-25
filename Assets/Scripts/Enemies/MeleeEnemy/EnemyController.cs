using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
  [Header("Component References")]
  private EnemyStats enemyStats;
  private Rigidbody2D rb;
  private EnemyKnockback knockbackComponent;

  [SerializeField] private bool isFacingRight = true;
  [SerializeField] private GameObject startPosition;
  private Vector2 leftPatrolPoint;
  private Vector2 rightPatrolPoint;
  private StateMachine stateMachine;
  private Dictionary<EnemyState, IState> states;
  private PlayerStats player;

  public bool IsFacingRight => isFacingRight;
  public EnemyStats EnemyStats => enemyStats;
  public PlayerStats Player => player;
  public Vector2 LeftPatrolPoint => leftPatrolPoint;
  public Vector2 RightPatrolPoint => rightPatrolPoint;

  public bool CanPerformActions => knockbackComponent == null || knockbackComponent.CanPerformActions();

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    enemyStats = GetComponent<EnemyStats>();
    player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

    knockbackComponent = GetComponent<EnemyKnockback>();
    if (knockbackComponent == null)
    {
      knockbackComponent = gameObject.AddComponent<EnemyKnockback>();
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

    leftPatrolPoint = (Vector2)startPosition.transform.position + Vector2.left * enemyStats.PatrolDistance;
    rightPatrolPoint = (Vector2)startPosition.transform.position + Vector2.right * enemyStats.PatrolDistance;
  }

  private void Update()
  {
    stateMachine.Update();
  }

  private void FixedUpdate()
  {
    stateMachine.FixedUpdate();
  }

  public void SetPlayer(PlayerStats player) => this.player = player;

  public bool IsPlayerInAttackRange()
  {
    if (player == null)
      return false;

    float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
    bool inRange = distanceToPlayer <= enemyStats.AttackRange;

    return inRange;
  }

  public void MoveTowards(Vector2 targetPosition)
  {
    if (knockbackComponent != null && !knockbackComponent.CanMove) return;

    transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemyStats.MoveSpeed * Time.fixedDeltaTime);
  }

  public void ChangeDirection(bool facingRight)
  {
    isFacingRight = facingRight;
    transform.localScale = new Vector3(isFacingRight ? 1f : -1f, transform.localScale.y, transform.localScale.z);
  }

  public IState GetState(EnemyState state) => states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;
}
