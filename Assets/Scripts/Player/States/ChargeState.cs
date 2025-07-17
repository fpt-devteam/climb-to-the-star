using UnityEngine;

public class ChargeState : BasePlayerState
{
  private Animator animator;
  private PlayerStats playerStats;

  public ChargeState(PlayerController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    playerStats = context.PlayerStats;
  }

  public override void Enter()
  {
    animator.Play("Charge");
  }

  public override void FixedUpdate()
  {
    playerStats.ChargeStamina();
  }

  public override IState CheckTransitions()
  {
    if (context.IsJumping())
    {
      return context.GetState(PlayerState.Air); // Use unified AirState
    }

    if (context.IsWalking())
    {
      return context.GetState(PlayerState.Locomotion);
    }

    if (context.IsIdling())
    {
      return context.GetState(PlayerState.Locomotion);
    }

    if (context.IsShielding())
    {
      return context.GetState(PlayerState.Shield);
    }

    if (context.IsDashing())
    {
      return context.GetState(PlayerState.Dash);
    }

    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    return null;
  }
}
