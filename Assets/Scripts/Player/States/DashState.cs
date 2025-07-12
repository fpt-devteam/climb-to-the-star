// using System.Collections;
// using UnityEngine;

// public class DashState : BasePlayerState
// {
//     private Animator animator;
//     private Rigidbody2D rb;
//     private TrailRenderer trailRenderer;
//     private float dashForce = 40f; // Adjust this value as needed

//     public DashState(PlayerController playerController)
//         : base(playerController)
//     {
//         animator = playerController.GetComponent<Animator>();
//         rb = playerController.GetComponent<Rigidbody2D>();
//         trailRenderer = playerController.GetComponentInChildren<TrailRenderer>();
//     }

//     public override void OnEnter()
//     {
//         animator.Play("Dash");
//         if (trailRenderer != null)
//         {
//             trailRenderer.Clear();
//             trailRenderer.enabled = true;
//             trailRenderer.emitting = true;
//         }
//         playerController.StartCoroutine(HandleDashCoroutine());
//         playerController.StartCoroutine(DisableTrailAfterDash());
//         playerController.StartCoroutine(ExitToDefault());
//         Debug.Log("Dash");
//     }

//     private IEnumerator DisableTrailAfterDash()
//     {
//         yield return new WaitForSeconds(0.6f);
//         if (trailRenderer != null)
//         {
//             trailRenderer.enabled = false;
//             trailRenderer.emitting = false;
//         }
//     }

//     private IEnumerator ExitToDefault()
//     {
//         yield return new WaitForSeconds(1f);
//     }

//     private IEnumerator HandleDashCoroutine()
//     {
//         yield return new WaitForSeconds(Random.Range(0.4f, 0.45f));
//         float direction = playerController.IsFacingRight() ? 1f : -1f;
//         rb.linearVelocity = new Vector2(direction * dashForce, rb.linearVelocity.y);
//         yield return new WaitForSeconds(0.1f);
//         rb.linearVelocity = new Vector2(direction, rb.linearVelocity.y); // Stop horizontal movement after dash
//         playerController.stateMachine.SetState(new IdleState(playerController));
//     }
// }
