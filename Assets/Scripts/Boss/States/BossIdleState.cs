using UnityEngine;

public class BossIdleState : BaseBossState
{
  private readonly Animator animator;

  public BossIdleState(BossController context) : base(context)
  {
    animator = context.Animator;
  }

  public override void Enter()
  {
    Debug.Log("Boss entering Idle State");
    animator.Play("Idle");
  }

  public override void Exit()
  {
    animator.StopPlayback();
  }

  public override IState CheckTransitions()
  {

    if (context.IsPlayerInMeleeAttackRange())
    {
      return context.GetState(BossState.MeleeAttack);
    }

    return context.GetState(BossState.Search);

    // if (context.IsInEnrageState())
    // {
    //   return context.GetState(BossState.Enrage);
    // }

    // return null;
  }
}
