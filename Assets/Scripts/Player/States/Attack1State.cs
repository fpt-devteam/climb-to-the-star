using UnityEngine;

public class Attack1State : BasePlayerAttackState
{
  public Attack1State(PlayerController context)
      : base(context)
  {
    animationName = "Attack_1";
    animationDuration = 0.4f;       // USER SPECIFIED: 400ms exact duration
    minAnimationDisplayTime = 0.25f; // REDUCED: Must be less than dash cancel time (70% of 0.4s = 0.28s)
    comboWindow = 0.3f;              // Combo window opens before animation ends
    earlyComboWindow = 0.15f;        // Early buffer for speed
    attackRange = 0.5f;
    damageMultiplier = 1.0f;

    // DEAD CELLS: Light attack - early dash cancel for fast combat
    dashCancelPercentage = 0.7f; // Allow dash after 70% for light attack
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Light attack allows more movement
    return 0.4f;
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
    // DEAD CELLS: Priority 0 - Dash cancel (highest priority for fluid combat)
    if (CanDashCancel())
    {
      Debug.Log("Attack1 -> Dash cancel triggered!");
      return context.GetState(PlayerState.Dash);
    }

    // Priority 1: Hurt state (highest priority)
    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    // Priority 2: Combo to next attack
    if (CanTransitionToNextAttack())
    {
      Debug.Log("Attack1 -> Attack2 combo triggered");
      return context.GetState(PlayerState.Attack2);
    }

    // Priority 3: Attack complete, return to locomotion
    if (IsAttackComplete())
    {
      Debug.Log("Attack1 complete, returning to locomotion");
      return context.GetState(PlayerState.Locomotion);
    }

    // Stay in current state
    return null;
  }
}
