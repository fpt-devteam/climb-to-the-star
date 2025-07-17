using System.Collections;
using UnityEngine;

public class DashState : BasePlayerState
{
  private Animator animator;
  private Rigidbody2D rb;
  private BoxCollider2D boxCollider;
  private TrailRenderer trailRenderer;
  private PlayerGhostTrail ghostTrail;
  private IPlayerMovement movement;

  private bool dashCompleted = false;
  private float dashDuration = 0.2f;
  private float dashTimer = 0f;
  private Vector2 dashDirection;
  private float dashSpeed;

  // DEAD CELLS: Dash through enemies with invincibility
  private bool isDashingThroughEnemies = false;
  private float dashInvincibilityDuration = 0.25f; // Slightly longer than dash for safety

  public DashState(PlayerController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    rb = context.GetComponent<Rigidbody2D>();
    boxCollider = context.GetComponent<BoxCollider2D>();
    trailRenderer = context.GetComponentInChildren<TrailRenderer>();
    ghostTrail = context.GetComponent<PlayerGhostTrail>();
    movement = context.PlayerMovement;
  }

  public override void Enter()
  {
    dashCompleted = false;
    dashTimer = 0f;
    animator.Play("Dash");

    // Start dash cooldown
    context.StartDashCooldown();

    // Determine dash direction based on facing direction
    dashDirection = context.IsFacingRight ? Vector2.right : Vector2.left;
    dashSpeed = context.PlayerStats.DashForce;

    // DEAD CELLS: Enable dash invincibility (can pass through enemies)
    isDashingThroughEnemies = true;
    context.PlayerStats.SetDashInvincibility(dashInvincibilityDuration);
    boxCollider.excludeLayers = LayerMask.GetMask("Enemy");

    // Set up external velocity control for clean dash
    movement.SetExternalVelocityControl(dashDuration + 0.1f);
    movement.SetExternalVelocity(dashDirection * dashSpeed);

    Debug.Log($"Dash Starting - Direction: {dashDirection}, Speed: {dashSpeed}, Invincible: {isDashingThroughEnemies}");
    Debug.Log($"Dash State - External control set for: {dashDuration + 0.1f}s");

    // Enable ghost trail effect for dash
    if (ghostTrail != null)
    {
      ghostTrail.setSpawnGhost(true);
      Debug.Log("Ghost trail enabled for dash");
    }

    // Enable trail renderer
    if (trailRenderer != null)
    {
      trailRenderer.Clear();
      trailRenderer.enabled = true;
      trailRenderer.emitting = true;
    }
  }

  public override void FixedUpdate()
  {
    dashTimer += Time.fixedDeltaTime;

    Debug.Log($"DashState FixedUpdate - dashTimer: {dashTimer:F3}, dashCompleted: {dashCompleted}, underExternalControl: {movement.IsUnderExternalControl()}");

    // Apply the dash movement - call Move with 0 input to trigger external movement
    movement.Move(0f);

    // Complete dash after duration
    if (dashTimer >= dashDuration && !dashCompleted)
    {
      Debug.Log("Dash duration completed - setting dashCompleted to true");
      dashCompleted = true;
      boxCollider.excludeLayers = LayerMask.GetMask("Default");
    }
  }

  public override void Exit()
  {
    Debug.Log("Dash Exited - Clean transition");

    // DEAD CELLS: End dash invincibility
    isDashingThroughEnemies = false;
    // Note: PlayerStats will handle invincibility timer automatically

    // Disable ghost trail effect
    if (ghostTrail != null)
    {
      ghostTrail.setSpawnGhost(false);
      Debug.Log("Ghost trail disabled after dash");
    }

    // Clean up trail effect
    if (trailRenderer != null)
    {
      trailRenderer.enabled = false;
      trailRenderer.emitting = false;
    }

    // Clear external velocity control
    movement.ClearExternalVelocityControl();
  }

  public override IState CheckTransitions()
  {
    // DEAD CELLS: Allow chaining from dash to other air actions
    if (dashCompleted)
    {
      Debug.Log($"Dash completed - checking transitions. Grounded: {context.IsGrounded}, Falling: {context.IsFalling()}");

      // CRITICAL FIX: Remove the problematic dash chaining check that was causing infinite loops
      // The old Priority 1 was: if (!context.IsGrounded && context.IsDashing()) - this caused infinite dash loops
      // Instead, we prioritize proper state transitions after dash completion

      // Priority 1: Air jump after dash (highest priority for air actions)
      if (context.IsJumping())
      {
        Debug.Log("DashState: Transitioning to jump after dash");
        return context.GetState(PlayerState.Air); // Use unified AirState
      }

      // Priority 2: Hurt state
      if (context.IsHurt())
      {
        Debug.Log("DashState: Transitioning to hurt");
        return context.GetState(PlayerState.Hurt);
      }

      // Priority 3: Landing - only if actually grounded
      if (context.IsGrounded)
      {
        Debug.Log("DashState: Landing - transitioning to Locomotion");
        return context.GetState(PlayerState.Locomotion);
      }

      // Priority 4: Still in air after dash - transition to air state (FIXED: This was Priority 5, now Priority 4)
      if (!context.IsGrounded)
      {
        Debug.Log("DashState: Still in air after dash - transitioning to Air state");
        return context.GetState(PlayerState.Air);
      }

      // Fallback: should not reach here, but return locomotion if somehow grounded
      Debug.Log("DashState: Fallback to Locomotion");
      return context.GetState(PlayerState.Locomotion);
    }

    return null;
  }
}
