using System.Collections;
using UnityEngine;

/// <summary>
/// Attack3 - SPIN ATTACK
/// Duration: 766ms + 150ms grace period
/// Part of professional Dead Cells-style combo chain system
/// </summary>
public class Attack3State : BasePlayerAttackState
{
  public Attack3State(PlayerController context) : base(context)
  {
    // SPIN ATTACK specifications
    animationName = "Attack_3";
    animationDuration = 0.766f;        // 766ms as requested
    comboGracePeriod = 0.15f;          // 150ms grace period
    earlyComboWindow = animationDuration * 0.7f;  // Later combo window for spin
    attackRange = 1.2f;                // Spin has wide range
    damageMultiplier = 1.3f;           // Higher damage for area attack

    // Professional timing
    minAnimationDisplayTime = animationDuration * 0.7f;    // Longer display for satisfying spin
    dashCancelPercentage = 0.6f;       // Later dash cancel for commitment

    Debug.Log($"Attack3 (SPIN) initialized - Duration: {animationDuration}s, Grace: {comboGracePeriod}s");
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Spin attack locks movement for commitment
    return 0.2f; // 20% movement speed
  }

  protected override float GetKnockbackMultiplier()
  {
    // Spin attack has strong knockback
    return damageMultiplier * 1.5f;
  }

  protected override void ApplyScreenShake()
  {
    // Spin attack creates medium screen shake
    CameraShake.MediumHit();
  }

  public override IState CheckTransitions()
  {
    // Professional combo system - check after minimum animation time
    if (hasMinAnimationTimePassed)
    {
      // COMBO CHAIN: Attack3 -> Attack4 (Spin -> Special)
      if (CanTransitionToNextAttack())
      {
        Debug.Log($"COMBO CHAIN: Attack3 (Spin) -> Attack4 (Special) at {stateTimer:F3}s");
        return context.GetState(PlayerState.Attack4);
      }

      // DASH CANCEL: Allow dash after 80% of animation
      if (CanDashCancel() && context.IsDashing())
      {
        Debug.Log($"DASH CANCEL: Attack3 (Spin) canceled at {stateTimer:F3}s");
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
