using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private BossStats bossStats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;

    private StateMachine stateMachine;
    private Dictionary<BossState, IState> states;

    private Vector2 lastKnownPlayerPosition;
    private float lastPlayerSeenTime;
    private bool isPlayerVisible;
    private float nextDecisionTime;
    private float nextPathfindingTime;

    // Properties
    public BossStats BossStats => bossStats;
    public Rigidbody2D Rigidbody => rb;
    public Animator Animator => animator;
    public Transform PlayerTransform => playerTransform;
    public Vector2 LastKnownPlayerPosition => lastKnownPlayerPosition;
    public float LastPlayerSeenTime => lastPlayerSeenTime;
    public bool IsPlayerVisible => isPlayerVisible;
    public bool IsPlayerInRange => IsPlayerInAttackRange() || IsPlayerInDetectionRange();
    public bool IsPlayerInMeleeRange => IsPlayerInMeleeAttackRange();
    public bool IsPlayerInRangedRange => IsPlayerInRangedAttackRange();

    private void Awake()
    {
        InitializeComponents();
        InitializeStateMachine();
        InitializeAIComponents();
    }

    private void InitializeComponents()
    {
        if (bossStats == null)
            bossStats = GetComponent<BossStats>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void InitializeStateMachine()
    {
        states = new Dictionary<BossState, IState>
        {
            { BossState.Idle, new BossIdleState(this) },
            { BossState.Search, new BossSearchState(this) },
            { BossState.Attack, new BossMeleeAttackState(this) },
            { BossState.Enrage, new BossEnrageState(this) },
            { BossState.Death, new BossDeathState(this) },
            { BossState.Hurt, new BossHurtState(this) },
            { BossState.RangedAttack, new BossRangedAttackState(this) },
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
        UpdatePlayerTracking();
        UpdateAI();
        stateMachine.FixedUpdate();
    }

    private void UpdatePlayerTracking()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool wasPlayerVisible = isPlayerVisible;

        // Check if player is in detection range
        if (distanceToPlayer <= bossStats.DetectionRange)
        {
            // Check line of sight
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                directionToPlayer,
                bossStats.DetectionRange,
                obstacleLayerMask
            );

            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                isPlayerVisible = true;
                lastKnownPlayerPosition = playerTransform.position;
                lastPlayerSeenTime = Time.time;
            }
            else
            {
                isPlayerVisible = false;
            }
        }
        else
        {
            isPlayerVisible = false;
        }

        // Debug visualization
        if (wasPlayerVisible != isPlayerVisible)
        {
            Debug.Log($"Boss: Player visibility changed to {isPlayerVisible}");
        }
    }

    private void UpdateAI()
    {
        if (Time.time >= nextDecisionTime)
        {
            decisionMaker.UpdateDecision();
            nextDecisionTime = Time.time + decisionUpdateRate;
        }

        if (Time.time >= nextPathfindingTime)
        {
            pathfinding.UpdatePathfinding();
            nextPathfindingTime = Time.time + pathfindingUpdateRate;
        }
    }

    // Player Detection Methods
    public bool IsPlayerInDetectionRange()
    {
        if (playerTransform == null)
            return false;
        return Vector2.Distance(transform.position, playerTransform.position)
            <= bossStats.DetectionRange;
    }

    public bool IsPlayerInAttackRange()
    {
        return IsPlayerInMeleeRange || IsPlayerInRangedRange;
    }

    public bool IsPlayerInMeleeAttackRange()
    {
        if (playerTransform == null)
            return false;
        return Vector2.Distance(transform.position, playerTransform.position)
            <= bossStats.MeleeAttackRange;
    }

    public bool IsPlayerInRangedAttackRange()
    {
        if (playerTransform == null)
            return false;
        return Vector2.Distance(transform.position, playerTransform.position)
            <= bossStats.RangeAttackRange;
    }

    public void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * bossStats.MoveSpeed;
    }

    private void HandleFacingDirection()
    {
        var direction = isFacingRight ? 1 : -1;
        transform.localScale = new Vector3(direction, 1, 1);
    }

    public void StopMovement() => rb.linearVelocity = Vector2.zero;

    public IState GetState(BossState state) => states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

    public void ChangeState(BossState newState)
    {
        IState state = GetState(newState);
        if (state != null)
        {
            stateMachine.ChangeState(state);
        }
    }
}
