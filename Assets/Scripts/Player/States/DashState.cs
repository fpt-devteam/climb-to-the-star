using System.Collections;
using UnityEngine;

public class DashState : BasePlayerState
{
    private Animator animator;
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private bool hasAppliedDash = false;

    public DashState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        rb = context.GetComponent<Rigidbody2D>();
        trailRenderer = context.GetComponentInChildren<TrailRenderer>();
    }

    public override void Enter()
    {
        hasAppliedDash = false;
        animator.Play("Dash");

        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
            trailRenderer.emitting = true;
        }
        context.StartCoroutine(HandleDashCoroutine());

        context.StartCoroutine(DisableTrailAfterDash());

        context.StartCoroutine(ExitToDefault());
    }

    private IEnumerator DisableTrailAfterDash()
    {
        yield return new WaitForSeconds(0.6f);
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
            trailRenderer.emitting = false;
        }
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator HandleDashCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0.4f, 0.45f));

        float direction = context.IsFacingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direction * context.PlayerStats.DashForce,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(0.1f);

        rb.linearVelocity = new Vector2(direction, rb.linearVelocity.y);

        hasAppliedDash = true;
    }

    public override IState CheckTransitions()
    {
        if (hasAppliedDash)
        {
            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
