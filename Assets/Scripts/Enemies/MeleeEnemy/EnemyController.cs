// using UnityEngine;

// public class EnemyController : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     [SerializeField]
//     private float moveSpeed = 2f;

//     [SerializeField]
//     private GameObject attackPoint;
//     public GameObject AttackPoint => attackPoint;

//     public StateMachine stateMachine;

//     private Enemy enemy;
//     private Player player;

//     // Cached states to avoid recreation
//     private EnemyPatrolState patrolState;
//     private EnemyHurtState hurtState;
//     private EnemyDieState dieState;
//     private EnemyAttackState attackState;

//     // Movement control
//     private bool canMove = true;
//     private Vector2 targetPosition;

//     public void SetPlayer(Player player)
//     {
//         this.player = player;
//     }

//     public Player GetPlayer()
//     {
//         return player;
//     }

//     private void Awake()
//     {
//         stateMachine = new StateMachine();
//         enemy = GetComponent<Enemy>();

//         // Create states once
//         patrolState = new EnemyPatrolState(this);
//         hurtState = new EnemyHurtState(this);
//         dieState = new EnemyDieState(this);
//         attackState = new EnemyAttackState(this);

//         // Set up proper transitions
//         SetupStateTransitions();

//         // Start with patrol state
//         stateMachine.SetState(patrolState);
//     }

//     private void SetupStateTransitions()
//     {
//         // Patrol state transitions
//         stateMachine.AddTransition(
//             patrolState,
//             attackState,
//             new FuncPredicate(() => player != null && IsPlayerInAttackRange())
//         );
//         stateMachine.AddTransition(
//             patrolState,
//             hurtState,
//             new FuncPredicate(() => enemy.IsHurt() && !enemy.IsDead())
//         );
//         stateMachine.AddTransition(patrolState, dieState, new FuncPredicate(() => enemy.IsDead()));

//         // Attack state transitions
//         stateMachine.AddTransition(
//             attackState,
//             patrolState,
//             new FuncPredicate(() => player == null || !IsPlayerInAttackRange())
//         );
//         stateMachine.AddTransition(
//             attackState,
//             hurtState,
//             new FuncPredicate(() => enemy.IsHurt() && !enemy.IsDead())
//         );
//         stateMachine.AddTransition(attackState, dieState, new FuncPredicate(() => enemy.IsDead()));

//         // Hurt state transitions
//         stateMachine.AddTransition(
//             hurtState,
//             patrolState,
//             new FuncPredicate(() => !enemy.IsHurt() && !enemy.IsDead())
//         );
//         stateMachine.AddTransition(hurtState, dieState, new FuncPredicate(() => enemy.IsDead()));

//         // Die state has no transitions (final state)
//     }

//     private bool IsPlayerInAttackRange()
//     {
//         if (player == null)
//             return false;

//         float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
//         return distanceToPlayer <= 2f; // Adjust based on your attack range
//     }

//     private void Update()
//     {
//         stateMachine.Update();
//     }

//     private void FixedUpdate()
//     {
//         stateMachine.FixedUpdate();

//         // Handle movement if allowed
//         if (canMove)
//         {
//             MoveTowards(targetPosition);
//         }
//     }

//     public void MoveTowards(Vector2 targetPosition)
//     {
//         this.targetPosition = targetPosition;

//         if (!canMove)
//             return;

//         Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

//         transform.position = Vector2.MoveTowards(
//             transform.position,
//             targetPosition,
//             moveSpeed * Time.fixedDeltaTime
//         );

//         // Flip sprite based on direction
//         if (direction.x != 0)
//         {
//             transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
//         }
//     }

//     public void StopMoving()
//     {
//         canMove = false;
//     }

//     public void ResumeMoving()
//     {
//         canMove = true;
//     }

//     public bool IsMoving()
//     {
//         return canMove;
//     }
// }
