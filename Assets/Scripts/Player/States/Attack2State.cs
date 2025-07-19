using System.Collections;
using UnityEngine;

/// <summary>
/// Attack2 - THRUST ATTACK
/// Duration: 666.66ms + 150ms grace period
/// Part of professional Dead Cells-style combo chain system
/// </summary>
public class Attack2State : BasePlayerAttackState
{
  public Attack2State(PlayerController context) : base(context)
  {
    // THRUST ATTACK specifications
    animationName = "Attack_2";
    animationDuration = 0.66666f;      // 666.66ms as requested
    comboGracePeriod = 0.15f;          // 150ms grace period
    earlyComboWindow = animationDuration * 0.7f;  // Earlier combo window for fast thrust
    attackRange = 1.0f;                // Thrust has longer range
    damageMultiplier = 1.0f;           // Standard damage

    // Professional timing
    minAnimationDisplayTime = animationDuration * 0.7f;    // Faster animation, shorter min display
    dashCancelPercentage = 0.6f;       // Earlier dash cancel for fast attack

    Debug.Log($"Attack2 (THRUST) initialized - Duration: {animationDuration}s, Grace: {comboGracePeriod}s");
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Thrust attack moves forward, allow good movement
    return 0.6f; // 60% movement speed
  }

  protected override float GetAttackRange()
  {
    // Attack2 has longer range
    return attackRange;
  }

  protected override float GetKnockbackMultiplier()
  {
    // Medium attack - moderate knockback
    return 1.2f;
  }

  protected override void ApplyScreenShake()
  {
    // Medium impact shake
    CameraShake.MediumHit();
  }

  public override IState CheckTransitions()
  {
    // Professional combo system - check after minimum animation time
    if (hasMinAnimationTimePassed)
    {
      // COMBO CHAIN: Attack2 -> Attack3 (Thrust -> Spin)
      if (CanTransitionToNextAttack())
      {
        Debug.Log($"COMBO CHAIN: Attack2 (Thrust) -> Attack3 (Spin) at {stateTimer:F3}s");
        return context.GetState(PlayerState.Attack3);
      }

      // DASH CANCEL: Allow dash after 70% of animation
      if (CanDashCancel() && context.IsDashing())
      {
        Debug.Log($"DASH CANCEL: Attack2 (Thrust) canceled at {stateTimer:F3}s");
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
