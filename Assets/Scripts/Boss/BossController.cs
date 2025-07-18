using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
  private Transform playerTransform;
  private BossStats bossStats;
  private Rigidbody2D rb;
  private Animator animator;

  private StateMachine stateMachine;
  private Dictionary<BossState, IState> states;
  private bool isFacingLeft = true;

  public BossStats BossStats => bossStats;
  public Rigidbody2D Rigidbody => rb;
  public Animator Animator => animator;
  public Transform PlayerTransform => playerTransform;

  private void Awake()
  {
    bossStats = GetComponent<BossStats>();
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();

    states = new Dictionary<BossState, IState>
        {
            { BossState.Idle, new BossIdleState(this) },
            { BossState.Search, new BossSearchState(this) },
            { BossState.MeleeAttack, new MeleeAttackState(this) },
            { BossState.Hurt, new BossHurtState(this) },
            { BossState.Death, new BossDeathState(this) }
        };

    stateMachine = new StateMachine();
    stateMachine.Initialize(GetState(BossState.Idle));
  }

  private void Update()
  {
    stateMachine.Update();
  }

  private void FixedUpdate()
  {
    playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    HandleFacingDirection();
    stateMachine.FixedUpdate();
    Debug.Log("Is in Melee Attack Range: " + IsPlayerInMeleeAttackRange());
  }

  public bool IsPlayerInMeleeAttackRange()
  {
    if (playerTransform == null)
    {
      Debug.LogWarning("Player Transform is not set.");
      return false;
    }

    var distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
    return distanceToPlayer <= bossStats.MeleeAttackRange;
  }

  public void MoveTowards(Vector2 target)
  {
    Vector2 direction = (target - (Vector2)transform.position).normalized;
    rb.linearVelocity = direction * bossStats.MoveSpeed;
  }

  public IState GetState(BossState state) => states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

  public bool IsInEnrageState() => bossStats.IsEnraged;

  public bool IsHurt() => bossStats.IsHurt;

  public void SetFacingDirection(bool isFacingLeft) => this.isFacingLeft = isFacingLeft;

  private void HandleFacingDirection()
  {
    var direction = isFacingLeft ? 1 : -1;
    transform.localScale = new Vector3(direction, 1, 1);
  }

  private void OnDrawGizmos()
  {
    if (bossStats != null)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(bossStats.MeleeAttackPoint.transform.position, bossStats.MeleeAttackRange);
    }
  }
}
