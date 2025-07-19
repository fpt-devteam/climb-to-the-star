using System.Collections;
using UnityEngine;

/// <summary>
/// Attack4 - SPECIAL ATTACK
/// Duration: 833.33ms + 150ms grace period
/// Final attack in professional Dead Cells-style combo chain system
/// </summary>
public class Attack4State : BasePlayerAttackState
{
  public Attack4State(PlayerController context) : base(context)
  {
    // SPECIAL ATTACK specifications
    animationName = "Attack_4";
    animationDuration = 0.83333f;      // 833.33ms as requested
    comboGracePeriod = 0.15f;          // 150ms grace period
    earlyComboWindow = animationDuration * 0.7f;  // Late combo window for special
    attackRange = 1.5f;                // Special has maximum range
    damageMultiplier = 2.0f;           // Highest damage for finisher

    // Professional timing
    minAnimationDisplayTime = animationDuration * 0.7f;    // Longest display for epic finisher
    dashCancelPercentage = 0.85f;      // Very late dash cancel for full commitment

    Debug.Log($"Attack4 (SPECIAL) initialized - Duration: {animationDuration}s, Grace: {comboGracePeriod}s");
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Special attack completely locks movement for maximum impact
    return 0.1f; // 10% movement speed
  }

  protected override float GetKnockbackMultiplier()
  {
    // Special attack has maximum knockback
    return damageMultiplier * 2.0f;
  }

  protected override void ApplyScreenShake()
  {
    // Special attack creates heavy screen shake
    CameraShake.HeavyHit();
  }

  protected override void ApplyImpactEffects(Vector3 hitPosition)
  {
    // Enhanced effects for special attack
    base.ApplyImpactEffects(hitPosition);

    // Additional visual effects for special attack
    Vector3 impactDirection = (hitPosition - attackPoint.transform.position).normalized;
    ImpactEffects.SimpleFlash(hitPosition, Color.red, 0.3f);
    ImpactEffects.HitSparks(hitPosition, impactDirection);

    // You could add particle effects, lighting, etc. here
    Debug.Log("SPECIAL ATTACK - Enhanced visual effects triggered!");
  }

  public override IState CheckTransitions()
  {
    // Professional combo system - check after minimum animation time
    if (hasMinAnimationTimePassed)
    {
      // NO MORE COMBOS: This is the final attack in the chain
      // Only allow dash cancel for escape options

      // DASH CANCEL: Allow dash after 85% of animation
      if (CanDashCancel() && context.IsDashing())
      {
        Debug.Log($"DASH CANCEL: Attack4 (Special) canceled at {stateTimer:F3}s");
        return context.GetState(PlayerState.Dash);
      }
    }

    // Standard transitions after full animation + grace period
    if (stateTimer >= animationDuration + comboGracePeriod)
    {
      Debug.Log("COMBO CHAIN COMPLETED - Special Attack finished!");

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
