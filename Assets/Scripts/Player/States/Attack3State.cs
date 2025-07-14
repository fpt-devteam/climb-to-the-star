using UnityEngine;

public class Attack3State : BasePlayerAttackState
{
    public Attack3State(PlayerController context)
        : base(context)
    {
        animationName = "Attack_3";
        animationDuration = 0.8f;
        comboWindow = 1.5f;
        attackRange = 0.7f;
        damageMultiplier = 1.4f;
    }

    public override IState CheckTransitions()
    {
        if (IsAnimationComplete())
        {
            if (CanTransitionToNextAttack())
            {
                return context.GetState(PlayerState.Attack4);
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
