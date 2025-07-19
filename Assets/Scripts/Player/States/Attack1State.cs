using System.Collections;
using UnityEngine;

/// <summary>
/// Attack1 - SMASH ATTACK
/// Duration: 733ms + 150ms grace period
/// Part of professional Dead Cells-style combo chain system
/// </summary>
public class Attack1State : BasePlayerAttackState
{
  public Attack1State(PlayerController context) : base(context)
  {
    // SMASH ATTACK specifications
    animationName = "Attack_1";
    animationDuration = 0.733f;        // 733ms as requested
    comboGracePeriod = 0.15f;          // 150ms grace period
    earlyComboWindow = animationDuration * 0.7f;
    attackRange = 0.8f;                // Smash has good range
    damageMultiplier = 1.2f;           // Slightly stronger base attack

    // Professional timing
    minAnimationDisplayTime = animationDuration * 0.7f;   // Must see animation for 250ms
    dashCancelPercentage = 0.6f;      // Can dash cancel after 75%

    Debug.Log($"Attack1 (SMASH) initialized - Duration: {animationDuration}s, Grace: {comboGracePeriod}s");
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Smash attack allows moderate movement for positioning
    return 0.4f; // 40% movement speed
  }

  protected override float GetKnockbackMultiplier()
  {
    // Light attack - minimal knockback for fast combos
    return 0.8f;
  }

  protected override void ApplyScreenShake()
  {
    // Light, quick shake for fast attacks
    CameraShake.LightHit();
  }

  public override IState CheckTransitions()
  {
    // Professional combo system - check after minimum animation time
    if (hasMinAnimationTimePassed)
    {
      // COMBO CHAIN: Attack1 -> Attack2 (Smash -> Thrust)
      if (CanTransitionToNextAttack())
      {
        Debug.Log($"COMBO CHAIN: Attack1 (Smash) -> Attack2 (Thrust) at {stateTimer:F3}s");
        return context.GetState(PlayerState.Attack2);
      }

      // DASH CANCEL: Allow dash after 75% of animation
      if (CanDashCancel() && context.IsDashing())
      {
        Debug.Log($"DASH CANCEL: Attack1 (Smash) canceled at {stateTimer:F3}s");
        return context.GetState(PlayerState.Dash);
      }
    }

    // Standard transitions after full animation + grace period
    if (stateTimer >= animationDuration + comboGracePeriod)
    {
      // High priority actions
      if (context.IsJumping())
      {
        return context.GetState(PlayerState.Air);
      }

      if (context.IsDashing())
      {
        return context.GetState(PlayerState.Dash);
      }

      // Return to appropriate movement state
      if (context.IsGrounded)
      {
        return context.GetState(PlayerState.Locomotion);
      }
      else
      {
        return context.GetState(PlayerState.Air);
      }
    }

    return null;
  }
}
