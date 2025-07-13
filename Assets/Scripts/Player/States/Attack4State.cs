using UnityEngine;

public class Attack4State : BasePlayerAttackState
{
    public Attack4State(PlayerController context)
        : base(context)
    {
        animationName = "Attack_4";
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
                return context.GetState(PlayerState.Attack1);
            }

            return context.GetState(PlayerState.Locomotion);
        }

        return null;
    }
}
