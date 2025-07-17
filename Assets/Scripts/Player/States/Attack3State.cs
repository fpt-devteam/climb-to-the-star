using UnityEngine;

public class Attack3State : BasePlayerAttackState
{
  public Attack3State(PlayerController context)
      : base(context)
  {
    animationName = "Attack_3";
    animationDuration = 0.6f;       // USER SPECIFIED: 600ms exact duration
    minAnimationDisplayTime = 0.45f; // REDUCED: Must be less than dash cancel time (80% of 0.6s = 0.48s)
    comboWindow = 0.48f;              // Combo window opens before animation ends
    earlyComboWindow = 0.25f;        // Early buffer for speed
    attackRange = 0.7f;              // Even longer reach
    damageMultiplier = 1.4f;         // Significant damage increase

    // DEAD CELLS: Heavy attack - later dash cancel for commitment
    dashCancelPercentage = 0.8f; // Allow dash after 80% for heavy attack
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Heavy attack - very limited movement
    return 0.15f;
  }

  protected override float GetAttackRange()
  {
    // Attack3 has the longest range so far
    return attackRange;
  }

  protected override float GetKnockbackMultiplier()
  {
    // Heavy attack - strong knockback
    return 1.5f;
  }

  protected override void ApplyScreenShake()
  {
    // Heavy impact shake
    CameraShake.HeavyHit();
  }

  public override IState CheckTransitions()
  {
    // DEAD CELLS: Priority 0 - Dash cancel (highest priority for fluid combat)
    if (CanDashCancel())
    {
      Debug.Log("Attack3 -> Dash cancel triggered!");
      return context.GetState(PlayerState.Dash);
    }

    // Priority 1: Hurt state (highest priority)
    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    // Priority 2: Combo to finisher attack
    if (CanTransitionToNextAttack())
    {
      Debug.Log("Attack3 -> Attack4 (FINISHER) combo triggered");
      return context.GetState(PlayerState.Attack4);
    }

    // Priority 3: Attack complete, return to locomotion
    if (IsAttackComplete())
    {
      Debug.Log("Attack3 complete, returning to locomotion");
      return context.GetState(PlayerState.Locomotion);
    }

    // Stay in current state
    return null;
  }
}
