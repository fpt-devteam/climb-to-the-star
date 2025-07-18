using System.Collections;
using UnityEngine;

/*
 * Professional Dead Cells-Style Combat System with Grace Period
 *
 * USER SPECIFICATIONS:
 * - Attack1: 400ms duration + 150ms grace period
 * - Attack2: 500ms duration + 150ms grace period
 * - Attack3: 600ms duration + 150ms grace period
 * - Attack4: 700ms duration + 150ms grace period
 *
 * GRACE PERIOD SYSTEM:
 * - After animation ends, player has an additional 150ms to continue combo
 * - This creates forgiving, responsive combat similar to Dead Cells
 * - Combo window = animation time + grace period for maximum responsiveness
 */

public abstract class BasePlayerAttackState : BasePlayerState
{
  [Header("Professional Combat System")]
  protected float animationDuration = 0.6f;
  protected float comboWindow = 0.8f; // Tighter window for precision
  protected float earlyComboWindow = 0.5f; // When player can start next attack
  protected float attackRange = 0.5f;
  protected float damageMultiplier = 1.0f;

  // NEW: Minimum animation display time to prevent skipping
  [Header("Animation Timing Control")]
  protected float minAnimationDisplayTime = 0.3f; // Minimum time animation must play

  // USER REQUESTED: Grace period after animation ends
  [Header("Combo Grace Period")]
  protected float comboGracePeriod = 0.15f; // 150ms grace period after animation ends

  // DEAD CELLS: Dash cancel system
  [Header("Dead Cells Dash Cancel")]
  protected float dashCancelPercentage = 0.75f; // Allow dash after 75% of animation
  protected bool canDashCancel = false; // Track if dash cancel is available

  // Professional timing system
  protected float stateTimer = 0f;
  protected float comboBufferTimer = 0f;
  protected float inputBufferDuration = 0.2f; // Input buffer window

  // State flags
  protected bool hasAppliedDamage = false;
  protected bool canCombo = false;
  protected bool hasInputBuffered = false;
  protected bool isInEarlyComboWindow = false;
  protected bool animationEventTriggered = false;
  protected bool hasMinAnimationTimePassed = false; // NEW: Prevents instant transitions

  // DASH CANCEL: Input buffering for more forgiving timing
  protected bool hasDashInputBuffered = false;
  protected float dashInputBufferTimer = 0f;
  protected float dashInputBufferDuration = 0.1f; // 100ms buffer for dash cancel

  protected GameObject attackPoint;
  protected Animator animator;
  protected string animationName;

  public BasePlayerAttackState(PlayerController context)
      : base(context)
  {
    attackPoint = context.PlayerStats.AttackPoint;
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    // Reset all state flags for clean entry
    ResetStateFlags();

    // Start animation and audio
    animator.Play(animationName);
    AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerAttack);

    // Apply damage immediately for responsive feel
    PerformDamage();

    Debug.Log($"Attack State Entered: {animationName}, MinDisplay: {minAnimationDisplayTime}s, Window: {comboWindow}s, Early: {earlyComboWindow}s");
  }

  public override void Update()
  {
    UpdateTimers();
    UpdateAnimationTiming(); // NEW: Check minimum animation time
    UpdateComboWindows();
    HandleInputBuffering();
  }

  public override void FixedUpdate()
  {
    // Professional movement during attacks (reduced mobility)
    HandleAttackMovement();
  }

  private void ResetStateFlags()
  {
    stateTimer = 0f;
    comboBufferTimer = 0f;
    hasAppliedDamage = false;
    canCombo = false;
    hasInputBuffered = false;
    isInEarlyComboWindow = false;
    animationEventTriggered = false;
    hasMinAnimationTimePassed = false; // NEW: Reset animation timing flag
    canDashCancel = false; // DEAD CELLS: Reset dash cancel availability

    // DASH CANCEL: Reset dash input buffer
    hasDashInputBuffered = false;
    dashInputBufferTimer = 0f;
  }

  private void UpdateTimers()
  {
    stateTimer += Time.deltaTime;

    if (comboBufferTimer > 0f)
    {
      comboBufferTimer -= Time.deltaTime;
    }

    // DASH CANCEL: Update dash input buffer
    if (dashInputBufferTimer > 0f)
    {
      dashInputBufferTimer -= Time.deltaTime;
      if (dashInputBufferTimer <= 0f)
      {
        hasDashInputBuffered = false;
      }
    }

    // DASH CANCEL: Buffer dash input for more forgiving timing
    if (context.PlayerInput.IsDashPressed())
    {
      hasDashInputBuffered = true;
      dashInputBufferTimer = dashInputBufferDuration;
      Debug.Log($"[DASH BUFFER] Dash input buffered at {stateTimer:F3}s - will last {dashInputBufferDuration}s");
    }
  }

  // NEW: Check if minimum animation display time has passed
  private void UpdateAnimationTiming()
  {
    if (!hasMinAnimationTimePassed && stateTimer >= minAnimationDisplayTime)
    {
      hasMinAnimationTimePassed = true;
      Debug.Log($"Minimum animation display time reached at {stateTimer:F2}s - transitions now allowed");
    }

    // DEAD CELLS: Check if dash cancel window is open
    float dashCancelTime = animationDuration * dashCancelPercentage;
    if (!canDashCancel && stateTimer >= dashCancelTime)
    {
      canDashCancel = true;
      Debug.Log($"DASH CANCEL WINDOW OPENED at {stateTimer:F2}s ({dashCancelPercentage * 100}% of {animationDuration}s animation)");
    }
  }

  private void UpdateComboWindows()
  {
    // Early combo window - can start buffering input (but only after min display time)
    if (hasMinAnimationTimePassed && stateTimer >= earlyComboWindow && !isInEarlyComboWindow)
    {
      isInEarlyComboWindow = true;
      Debug.Log($"Early combo window opened at {stateTimer:F2}s (after min display time)");
    }

    // Full combo window - can execute combo (but only after min display time)
    // Extended to include grace period after animation ends
    if (hasMinAnimationTimePassed && stateTimer >= comboWindow && !canCombo)
    {
      canCombo = true;
      Debug.Log($"Full combo window opened at {stateTimer:F2}s (includes grace period until {animationDuration + comboGracePeriod:F2}s)");
    }
  }

  private void HandleInputBuffering()
  {
    // Professional input buffering - register attack input during early window
    // MODIFIED: Only allow buffering after minimum animation time
    if (isInEarlyComboWindow && hasMinAnimationTimePassed && context.PlayerInput.IsAttackPressed())
    {
      hasInputBuffered = true;
      comboBufferTimer = inputBufferDuration;
      Debug.Log($"Attack input buffered at {stateTimer:F2}s (after min display time)");
    }
  }

  private void HandleAttackMovement()
  {
    // Allow limited movement during attacks for better game feel
    float movementInput = context.PlayerInput.GetMovementInput();
    float attackMovementMultiplier = GetAttackMovementMultiplier();

    if (!context.PlayerMovement.IsUnderExternalControl())
    {
      // Reduced movement during attacks
      context.PlayerMovement.Move(movementInput * attackMovementMultiplier);
    }
  }

  // Override in specific attacks for different movement feels
  protected virtual float GetAttackMovementMultiplier()
  {
    // Light attacks allow more movement, heavy attacks lock player more
    return 0.3f; // 30% movement speed during attacks
  }

  protected virtual void PerformDamage()
  {
    if (hasAppliedDamage) return;

    float baseDamage = context.PlayerStats.AttackDamage;
    float finalDamage = baseDamage * damageMultiplier;

    Collider2D[] colliders = Physics2D.OverlapCircleAll(
        attackPoint.transform.position,
        GetAttackRange(),
        LayerMask.GetMask("Enemy")
    );

    foreach (Collider2D collider in colliders)
    {
      EnemyStats enemyStats = collider.GetComponent<EnemyStats>();
      if (enemyStats != null)
      {
        enemyStats.TakeDamage(finalDamage);

        EnemyKnockback knockback = collider.GetComponent<EnemyKnockback>();
        if (knockback != null)
        {
          float knockbackMultiplier = GetKnockbackMultiplier();
          knockback.ApplyKnockback(attackPoint.transform.position, knockbackMultiplier);
        }

        ApplyScreenShake();
        ApplyImpactEffects(collider.transform.position);
      }

      BossStats bossStats = collider.GetComponent<BossStats>();
      if (bossStats != null)
      {
        bossStats.TakeDamage(finalDamage);

        EnemyKnockback knockback = collider.GetComponent<EnemyKnockback>();

        if (knockback != null)
        {
          float knockbackMultiplier = GetKnockbackMultiplier();
          knockback.ApplyKnockback(attackPoint.transform.position, knockbackMultiplier);
        }

        ApplyScreenShake();
        ApplyImpactEffects(collider.transform.position);
      }
    }

    hasAppliedDamage = true;
  }

  // Get knockback multiplier based on attack type (override in specific attacks)
  protected virtual float GetKnockbackMultiplier()
  {
    return damageMultiplier; // Scale knockback with damage
  }

  // Apply appropriate screen shake for attack type
  protected virtual void ApplyScreenShake()
  {
    // Default light shake - override in specific attacks for different intensities
    CameraShake.LightHit();
  }

  // Apply visual impact effects
  protected virtual void ApplyImpactEffects(Vector3 hitPosition)
  {
    // Calculate direction from attack point to hit position
    Vector3 impactDirection = (hitPosition - attackPoint.transform.position).normalized;

    // Create impact effect
    ImpactEffects.SimpleFlash(hitPosition, Color.yellow, 0.15f);

    // Hit sparks effect if available
    ImpactEffects.HitSparks(hitPosition, impactDirection);
  }

  // DEAD CELLS: Check if dash cancel is available
  protected bool CanDashCancel()
  {
    bool dashPressed = context.PlayerInput.IsDashPressed();
    bool hasDashInput = dashPressed || hasDashInputBuffered; // Include buffered input

    // COMPREHENSIVE DEBUG: Log every frame during dash cancel window
    if (canDashCancel)
    {
      Debug.Log($"[DASH CANCEL DEBUG] Timer: {stateTimer:F3}s, DashPressed: {dashPressed}, Buffered: {hasDashInputBuffered}, HasInput: {hasDashInput}, Window: {canDashCancel}, Cooldown: {context.IsDashOnCooldown}");
    }

    // CRITICAL FIX: Dash cancel should work based on dash cancel window, NOT minimum animation time
    // This allows responsive combat like Dead Cells where dash cancel is immediate when window opens
    bool inDashCancelWindow = canDashCancel && !context.IsDashOnCooldown;

    if (hasDashInput && canDashCancel && !context.IsDashOnCooldown)
    {
      Debug.Log($"DASH CANCEL TRIGGERED at {stateTimer:F2}s - canceling {animationName} (buffered input: {hasDashInputBuffered})");
      // Clear buffer after successful dash cancel
      hasDashInputBuffered = false;
      dashInputBufferTimer = 0f;
      return true;
    }
    else if (hasDashInput && !canDashCancel)
    {
      Debug.Log($"Dash cancel blocked - window not open yet ({stateTimer:F2}s < {animationDuration * dashCancelPercentage:F2}s)");
    }
    else if (hasDashInput && context.IsDashOnCooldown)
    {
      Debug.Log($"Dash cancel blocked - dash on cooldown");
    }

    return false;
  }

  // Professional combo detection - MODIFIED: Must pass minimum animation time + grace period
  protected bool CanTransitionToNextAttack()
  {
    // CRITICAL: Must have minimum animation time passed AND be in combo window AND have input
    bool minTimePassed = hasMinAnimationTimePassed;
    bool inComboWindow = canCombo;
    bool inGracePeriod = stateTimer <= (animationDuration + comboGracePeriod); // Grace period after animation
    bool hasValidInput = context.PlayerInput.IsAttackPressed() ||
                       (hasInputBuffered && comboBufferTimer > 0f);

    bool canComboNow = minTimePassed && (inComboWindow || inGracePeriod) && hasValidInput;

    if (canComboNow)
    {
      if (inGracePeriod && stateTimer > animationDuration)
      {
        Debug.Log($"GRACE PERIOD COMBO - Timer: {stateTimer:F2}s, Animation: {animationDuration}s, Grace: {comboGracePeriod}s");
      }
      else
      {
        Debug.Log($"Normal combo triggered - MinTime: {minTimePassed}, Window: {canCombo}, Input: {hasValidInput}, Buffer: {comboBufferTimer:F2}s");
      }
    }
    else if (hasValidInput && !minTimePassed)
    {
      Debug.Log($"Combo blocked - waiting for minimum animation time ({minAnimationDisplayTime}s), current: {stateTimer:F2}s");
    }
    else if (hasValidInput && stateTimer > (animationDuration + comboGracePeriod))
    {
      Debug.Log($"Combo window expired - Timer: {stateTimer:F2}s, Max allowed: {animationDuration + comboGracePeriod:F2}s");
    }

    return canComboNow;
  }

  protected bool IsAttackComplete()
  {
    // Attack is complete when animation duration + grace period is reached
    return stateTimer >= (animationDuration + comboGracePeriod);
  }

  // Dynamic attack range per attack type
  protected virtual float GetAttackRange()
  {
    return attackRange;
  }

  // Animation event support for precise timing
  public virtual void OnAttackAnimationEvent(string eventName)
  {
    switch (eventName)
    {
      case "DamageFrame":
        if (!hasAppliedDamage)
          PerformDamage();
        break;
      case "ComboWindow":
        // Animation event can force combo window open (but still respects min time)
        if (hasMinAnimationTimePassed)
        {
          canCombo = true;
          isInEarlyComboWindow = true;
        }
        break;
      case "AttackEnd":
        animationEventTriggered = true;
        break;
    }
  }

  protected virtual void OnDrawGizmosSelected()
  {
    if (attackPoint != null)
    {
      // Visual feedback for attack range
      Gizmos.color = hasAppliedDamage ? Color.red : Color.yellow;
      Gizmos.DrawWireSphere(attackPoint.transform.position, GetAttackRange());

      // Show combo timing windows
      if (Application.isPlaying)
      {
        // Green if in combo window or grace period
        bool inGracePeriod = stateTimer <= (animationDuration + comboGracePeriod) && stateTimer > animationDuration;
        bool canComboNow = (hasMinAnimationTimePassed && isInEarlyComboWindow) || inGracePeriod;

        Gizmos.color = canComboNow ? Color.green : Color.gray;
        Gizmos.DrawWireCube(attackPoint.transform.position + Vector3.up * 0.5f, Vector3.one * 0.1f);

        // Show grace period specifically
        if (inGracePeriod)
        {
          Gizmos.color = Color.cyan; // Cyan for grace period
          Gizmos.DrawWireCube(attackPoint.transform.position + Vector3.up * 0.7f, Vector3.one * 0.08f);
        }
      }
    }
  }
}
