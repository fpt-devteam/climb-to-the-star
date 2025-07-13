using UnityEngine;

public class Attack2State : BasePlayerAttackState
{
    public Attack2State(PlayerController context)
        : base(context)
    {
        animationName = "Attack_2";
        animationDuration = 0.7f;
        comboWindow = 1.5f;
        attackRange = 0.6f;
        damageMultiplier = 1.2f;
    }

    public override IState CheckTransitions()
    {
        if (IsAnimationComplete())
        {
            if (CanTransitionToNextAttack())
            {
                return context.GetState(PlayerState.Attack3);
            }

            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
