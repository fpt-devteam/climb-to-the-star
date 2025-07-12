// using System.Collections;
// using UnityEngine;

// public class AttackState : BasePlayerState
// {
//     private static readonly int[] AnimHashes =
//     {
//         Animator.StringToHash("Attack_1"),
//         Animator.StringToHash("Attack_2"),
//         Animator.StringToHash("Attack_3"),
//         Animator.StringToHash("Attack_4"),
//     };

//     private static readonly float[] Durations = { 0.5f, 0.5f, 0.5f, 0.5f };

//     private Animator animator;
//     private GameObject attackPoint;

//     private int attackIndex;
//     private float attackTimer;
//     private float comboWindow = 1f;
//     private float comboTimer = 0f;
//     private bool hasAppliedComboTimer = false;

//     public AttackState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//         attackPoint = playerController.AttackPoint;
//     }

//     public override void OnEnter()
//     {
//         // attackIndex = playerController.GetAttackId();
//         HandleAttack();
//     }

//     public override void OnExit() { }

//     private void HandleAttack()
//     {
//         PlayAttackAnimation();
//         DealDamageToEnemies();
//     }

//     private void PlayAttackAnimation()
//     {
//         animator.Play(AnimHashes[attackIndex]);
//         attackTimer = Durations[attackIndex];
//     }

//     private void DealDamageToEnemies()
//     {
//         Collider2D[] colliders = Physics2D.OverlapCircleAll(
//             attackPoint.transform.position,
//             0.5f,
//             LayerMask.GetMask("Enemy")
//         );
//         foreach (Collider2D collider in colliders)
//         {
//             Enemy enemy = collider.GetComponent<Enemy>();
//             if (enemy != null)
//             {
//                 enemy.DeductHealth(10f);
//                 Debug.Log("Enemy hit");
//             }
//         }
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(attackPoint.transform.position, 1.5f);
//     }
// }
