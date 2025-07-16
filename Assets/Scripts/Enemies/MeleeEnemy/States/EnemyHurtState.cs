using System.Collections;
using UnityEngine;

public class EnemyHurtState : BaseEnemyState
{
  [SerializeField]
  private float hurtDuration = 0.2f;

  private Animator animator;
  private bool isAppliedAnimation = false;

  public EnemyHurtState(EnemyController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    Debug.Log("Enemy entering Hurt State");
    isAppliedAnimation = false;
    animator.Play("Hurt");
    context.StartCoroutine(ExitHurtState());
  }

  private IEnumerator ExitHurtState()
  {
    yield return new WaitForSeconds(hurtDuration);
    isAppliedAnimation = true;
  }
  public override void Exit()
  {
    animator.StopPlayback();
  }

  public override IState CheckTransitions()
  {
    if (context.EnemyStats.IsDead)
    {
      return context.GetState(EnemyState.Die);
    }

    if (isAppliedAnimation)
    {
      return context.GetState(EnemyState.Patrol);
    }

    Debug.Log("Enemy is still hurt, staying in Hurt State");
    return null;
  }
}
