using UnityEngine;
public class BossSearchState : BaseBossState
{
  private readonly Animator animator;
  public BossSearchState(BossController context) : base(context)
  {
    animator = context.Animator;
  }

  public override void Enter()
  {
    Debug.Log("Boss entering Search State");
    animator.Play("Walk");
  }

  public override void Exit()
  {
    animator.StopPlayback();
  }

  public override void FixedUpdate()
  {
    var playerPosition = context.PlayerTransform.position;
    var direction = (playerPosition - context.transform.position).normalized;

    context.MoveTowards(playerPosition);

    var isFacingLeft = direction.x < 0;
    context.SetFacingDirection(isFacingLeft);
  }

  public override IState CheckTransitions()
  {
    if (context.IsPlayerInMeleeAttackRange())
    {
      return context.GetState(BossState.MeleeAttack);
    }

    if (context.BossStats.IsHurt)
    {
      return context.GetState(BossState.Hurt);
    }

    return null;
  }
}