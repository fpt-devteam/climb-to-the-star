// using System.Collections;
// using UnityEngine;

// public class EnemyDieState : BaseEnemyState
// {
//     [SerializeField]
//     private float deathAnimationDuration = 1f;

//     private Animator animator;
//     private Enemy enemy;

//     public EnemyDieState(EnemyController enemyController)
//         : base(enemyController)
//     {
//         animator = enemyController.GetComponent<Animator>();
//         enemy = enemyController.GetComponent<Enemy>();
//     }

//     public override void OnEnter()
//     {
//         Debug.Log("Enemy entering Die State");

//         // Call the enemy's die method
//         enemy.Die();

//         // Play death animation
//         animator.Play("Die");

//         // Stop all movement
//         enemyController.StopMoving();

//         // Start death sequence
//         enemyController.StartCoroutine(DeathSequence());
//     }

//     private IEnumerator DeathSequence()
//     {
//         yield return new WaitForSeconds(deathAnimationDuration);

//         // Optional: Destroy the enemy GameObject
//         // Destroy(enemyController.gameObject);

//         // Or disable the enemy
//         enemyController.gameObject.SetActive(false);
//     }

//     public override void OnExit()
//     {
//         Debug.Log("Enemy exiting Die State");
//     }
// }
