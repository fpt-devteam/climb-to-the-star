// using System.Collections;
// using UnityEngine;

// public class DieState : BasePlayerState
// {
//     private Animator animator;

//     public DieState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("Die");
//         playerController.StartCoroutine(ExitToDefault());
//         Debug.Log("Die");
//     }

//     private IEnumerator ExitToDefault()
//     {
//         yield return new WaitForSeconds(1f);
//         Time.timeScale = 0f;
//     }
// }
