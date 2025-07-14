using UnityEngine;

public class Attack1State : BasePlayerAttackState
{
    public Attack1State(PlayerController context)
        : base(context)
    {
        animationName = "Attack_1";
        animationDuration = 0.6f;
        comboWindow = 1.5f;
        attackRange = 0.5f;
        damageMultiplier = 1.0f;
    }

    public override IState CheckTransitions()
    {
        if (IsAnimationComplete())
        {
            if (CanTransitionToNextAttack())
            {
                return context.GetState(PlayerState.Attack2);
            }

            if (context.IsHurt())
            {
                return context.GetState(PlayerState.Hurt);
            }

            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
