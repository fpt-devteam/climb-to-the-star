using UnityEngine;

public class Attack2State : BasePlayerAttackState
{
  public Attack2State(PlayerController context)
      : base(context)
  {
    animationName = "Attack_2";
    animationDuration = 0.5f;        // USER SPECIFIED: 500ms exact duration
    minAnimationDisplayTime = 0.35f;  // REDUCED: Must be less than dash cancel time (75% of 0.5s = 0.375s)
    comboWindow = 0.38f;             // Combo window opens before animation ends
    earlyComboWindow = 0.2f;         // Early buffer for speed
    attackRange = 0.6f;              // Longer reach
    damageMultiplier = 1.2f;         // More damage

    // DEAD CELLS: Medium attack - standard dash cancel timing
    dashCancelPercentage = 0.75f; // Allow dash after 75% for medium attack
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Medium attack - less movement than light attack
    return 0.25f;
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
    // DEAD CELLS: Priority 0 - Dash cancel (highest priority for fluid combat)
    if (CanDashCancel())
    {
      Debug.Log("Attack2 -> Dash cancel triggered!");
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
      Debug.Log("Attack2 -> Attack3 combo triggered");
      return context.GetState(PlayerState.Attack3);
    }

    // Priority 3: Attack complete, return to locomotion
    if (IsAttackComplete())
    {
      Debug.Log("Attack2 complete, returning to locomotion");
      return context.GetState(PlayerState.Locomotion);
    }

    // Stay in current state
    return null;
  }
}
