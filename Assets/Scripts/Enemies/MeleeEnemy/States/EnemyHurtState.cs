// using System.Collections;
// using UnityEngine;

// public class EnemyHurtState : BaseEnemyState
// {
//     [SerializeField]
//     private float hurtDuration = 0.4f;

//     private Animator animator;
//     private Enemy enemy;

//     public EnemyHurtState(EnemyController enemyController)
//         : base(enemyController)
//     {
//         animator = enemyController.GetComponent<Animator>();
//         enemy = enemyController.GetComponent<Enemy>();
//     }

//     public override void OnEnter()
//     {
//         Debug.Log("Enemy entering Hurt State");

//         // Check if enemy should die instead
//         if (enemy.IsDead())
//         {
//             // State machine will handle transition to die state
//             return;
//         }

//         animator.Play("Hurt");
//         enemyController.StartCoroutine(ExitHurtState());
//     }

//     private IEnumerator ExitHurtState()
//     {
//         yield return new WaitForSeconds(hurtDuration);
//         // State machine will handle transition back to appropriate state
//     }

//     public override void OnExit()
//     {
//         Debug.Log("Enemy exiting Hurt State");
//     }
// }
