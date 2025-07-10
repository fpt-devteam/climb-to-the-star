using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float patrolDistance = 5f;

    private StateMachine stateMachine;
    private GameObject player;

    [Header("References")]
    [SerializeField]
    private LayerMask playerLayer = 1;

    [SerializeField]
    private LayerMask groundLayer = 1;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugGizmos = true;

    public StateMachine StateMachine => stateMachine;
    public Rigidbody2D Rigidbody => rb;
    public Animator Animator => animator;
    private Rigidbody2D rb;
    private Animator animator;

    public Vector2 startPosition;
    public Vector2 leftPatrolPoint;
    public Vector2 rightPatrolPoint;

    public bool isMovingRight = true;
    public bool test = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        startPosition = (Vector2)transform.position;
        leftPatrolPoint = startPosition + Vector2.left * patrolDistance;
        rightPatrolPoint = startPosition + Vector2.right * patrolDistance;

        stateMachine = new StateMachine();

        var patrolState = new EnemyPatrolState(this);
        var idleState = new EnemyIdleState(this);

        stateMachine.AddTransition(idleState, patrolState, new FuncPredicate(() => test));
        stateMachine.AddTransition(patrolState, idleState, new FuncPredicate(() => !test));

        stateMachine.SetState(idleState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }

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
