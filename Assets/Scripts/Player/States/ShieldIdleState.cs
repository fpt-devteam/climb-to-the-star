// using System.Collections;
// using UnityEngine;

// public class ShieldIdleState : BasePlayerState
// {
//     private Animator animator;

//     public ShieldIdleState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("ShieldIdle");
//         Debug.Log("ShieldIdle");
//         playerController.StartShield();
//     }

//     public override void OnExit()
//     {
//         playerController.StopShield();
//     }
// }
