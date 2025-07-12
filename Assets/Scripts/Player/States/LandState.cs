// using System.Collections;
// using UnityEngine;

// public class LandingState : BasePlayerState
// {
//     private Animator animator;

//     public LandingState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("Land");
//         playerController.StartCoroutine(ExitToDefault());
//         Debug.Log("Landing");
//     }

//     private IEnumerator ExitToDefault()
//     {
//         yield return new WaitForSeconds(0.4f);
//         playerController.stateMachine.SetState(new IdleState(playerController));
//     }
// }
