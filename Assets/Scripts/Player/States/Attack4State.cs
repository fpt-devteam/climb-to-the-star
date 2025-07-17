using UnityEngine;

public class Attack4State : BasePlayerAttackState
{
  public Attack4State(PlayerController context)
      : base(context)
  {
    animationName = "Attack_4";
    animationDuration = 0.7f;       // USER SPECIFIED: 700ms exact duration
    minAnimationDisplayTime = 0.55f; // REDUCED: Must be less than dash cancel time (85% of 0.7s = 0.595s)
    comboWindow = 0.58f;             // Combo window opens before animation ends
    earlyComboWindow = 0.3f;         // Early buffer for speed
    attackRange = 0.8f;              // Maximum reach
    damageMultiplier = 1.8f;         // Finisher damage

    // DEAD CELLS: Finisher attack - very late dash cancel for high commitment
    dashCancelPercentage = 0.85f; // Allow dash after 85% for finisher attack
  }

  protected override float GetAttackMovementMultiplier()
  {
    // Finisher attack - player is locked in place for impact
    return 0.05f; // Almost no movement
  }

  protected override float GetAttackRange()
  {
    // Attack4 has maximum range - finisher attack
    return attackRange;
  }

  protected override float GetKnockbackMultiplier()
  {
    // Finisher attack - maximum knockback
    return 2.0f;
  }

  protected override void ApplyScreenShake()
  {
    // Finisher impact - maximum screen shake
    CameraShake.FinisherAttack();
  }

  protected override void ApplyImpactEffects(Vector3 hitPosition)
  {
    // Finisher has enhanced visual effects
    Vector3 impactDirection = (hitPosition - context.PlayerStats.AttackPoint.transform.position).normalized;

    // Multiple effects for finisher
    ImpactEffects.SimpleFlash(hitPosition, Color.red, 0.3f);
    ImpactEffects.CriticalHit(hitPosition, impactDirection);
    ImpactEffects.HitSparks(hitPosition, impactDirection);

    Debug.Log("FINISHER IMPACT EFFECTS TRIGGERED!");
  }

  public override IState CheckTransitions()
  {
    // DEAD CELLS: Priority 0 - Dash cancel (highest priority for fluid combat)
    if (CanDashCancel())
    {
      Debug.Log("Attack4 -> Dash cancel triggered!");
      return context.GetState(PlayerState.Dash);
    }

    // Priority 1: Hurt state (highest priority)
    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    // Priority 2: SPECIAL - Loop back to Attack1 for infinite combo potential
    if (CanTransitionToNextAttack())
    {
      Debug.Log("Attack4 -> Attack1 (COMBO LOOP) triggered - Infinite combo potential!");
      return context.GetState(PlayerState.Attack1);
    }

    // Priority 3: Attack complete, return to locomotion
    if (IsAttackComplete())
    {
      Debug.Log("Attack4 (FINISHER) complete, returning to locomotion");
      return context.GetState(PlayerState.Locomotion);
    }

    // Stay in current state
    return null;
  }

  protected override void PerformDamage()
  {
    // Finisher attack has special effects
    base.PerformDamage();

    // DEAD CELLS: Extra visual feedback for finisher
    Debug.Log("FINISHER ATTACK EXECUTED - Maximum damage and knockback!");
  }
}
