// using System.Collections;
// using UnityEngine;

// public class VictoryState : BasePlayerState
// {
//     private Animator animator;

//     public VictoryState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("Victory");
//         playerController.StartCoroutine(ExitToDefault());
//         Debug.Log("Victory");
//     }

//     private IEnumerator ExitToDefault()
//     {
//         yield return new WaitForSeconds(0.4f);
//         playerController.stateMachine.SetState(new IdleState(playerController));
//     }
// }
