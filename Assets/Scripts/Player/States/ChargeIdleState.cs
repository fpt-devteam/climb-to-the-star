// using System.Collections;
// using UnityEngine;

// public class ChargeIdleState : BasePlayerState
// {
//     private Animator animator;

//     public ChargeIdleState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("ChargeIdle");
//         Debug.Log("ChargeIdle");
//     }

//     public override void FixedUpdate()
//     {
//         playerController.HandleCharge();
//     }
// }
