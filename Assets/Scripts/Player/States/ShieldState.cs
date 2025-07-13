using UnityEngine;

public class ShieldState : BasePlayerState
{
    private Animator animator;
    private PlayerStats playerStats;

    public ShieldState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
        playerStats = context.PlayerStats;
    }

    public override void Enter()
    {
        animator.Play("Shield");
    }

    public override void FixedUpdate()
    {
        playerStats.ChargeStamina();
    }

    public override IState CheckTransitions()
    {
        if (context.IsJumping())
        {
            return context.GetState(PlayerState.Locomotion);
        }

        if (context.IsWalking())
        {
            return context.GetState(PlayerState.Locomotion);
        }

        if (context.IsIdling())
        {
            return context.GetState(PlayerState.Locomotion);
        }

        if (context.IsCharging())
        {
            return context.GetState(PlayerState.Charge);
        }

        if (context.IsDashing())
        {
            return context.GetState(PlayerState.Dash);
        }

        return null;
    }
}
