// using System.Collections;
// using UnityEngine;

// public class EnemyAttackState : BaseEnemyState
// {
//   [SerializeField]
//   private float attackCooldown = 1.5f;

//   [SerializeField]
//   private float attackRange = 1.5f;

//   private Animator animator;
//   private GameObject attackPoint;
//   private float attackTimer;
//   private bool isAttacking;

//   public EnemyAttackState(EnemyController enemyController)
//       : base(enemyController)
//   {
//     animator = enemyController.GetComponent<Animator>();
//     attackPoint = enemyController.AttackPoint;
//   }

//   public override void OnEnter()
//   {
//     Debug.Log("Enemy entering Attack State");
//     attackTimer = 0f;
//     isAttacking = false;
//   }

//   public override void Update()
//   {
//     if (isAttacking)
//     {
//       attackTimer -= Time.deltaTime;
//     }
//   }

//   public override void FixedUpdate()
//   {
//     Player player = enemyController.GetPlayer();

//     if (player == null)
//     {
//       // No player to attack, state machine will transition back to patrol
//       return;
//     }

//     float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

//     if (distanceToPlayer <= attackRange)
//     {
//       // In attack range
//       if (!isAttacking && attackTimer <= 0f)
//       {
//         PerformAttack();
//       }
//     }
//     else
//     {
//       // Chase player
//       ChasePlayer();
//     }
//   }

//   private void ChasePlayer()
//   {
//     Player player = enemyController.GetPlayer();
//     if (player != null)
//     {
//       enemyController.MoveTowards(player.transform.position);
//     }
//   }

//   private void PerformAttack()
//   {
//     isAttacking = true;
//     attackTimer = attackCooldown;

//     // Stop moving during attack
//     enemyController.StopMoving();

//     // Play attack animation
//     animator.Play("Attack");

//     // Check for player in attack range and deal damage
//     Collider2D[] colliders = Physics2D.OverlapCircleAll(
//         attackPoint.transform.position,
//         0.5f,
//         LayerMask.GetMask("Player")
//     );

//     if (colliders.Length > 0)
//     {
//       foreach (Collider2D collider in colliders)
//       {
//         Player player = collider.GetComponent<Player>();
//         if (player != null)
//         {
//           player.DeductHealth(10f);
//           Debug.Log("Player hit by enemy attack");
//         }
//       }
//     }

//     // Start coroutine to reset attack state
//     enemyController.StartCoroutine(ResetAttackState());
//   }

//   private IEnumerator ResetAttackState()
//   {
//     // Wait for attack animation to complete
//     yield return new WaitForSeconds(0.5f);
//     isAttacking = false;
//   }

//   public override void OnExit()
//   {
//     Debug.Log("Enemy exiting Attack State");
//     isAttacking = false;
//   }

//   private void OnDrawGizmosSelected()
//   {
//     if (attackPoint != null)
//     {
//       Gizmos.color = Color.red;
//       Gizmos.DrawWireSphere(attackPoint.transform.position, 0.5f);
//     }

//     Gizmos.color = Color.yellow;
//     Gizmos.DrawWireSphere(transform.position, attackRange);
//   }
// }
