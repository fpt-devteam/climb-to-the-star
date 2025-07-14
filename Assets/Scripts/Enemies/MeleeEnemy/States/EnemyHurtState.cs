using System.Collections;
using UnityEngine;

public class EnemyHurtState : BaseEnemyState
{
  [SerializeField]
  private float hurtDuration = 0.4f;

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

    if (context.EnemyStats.IsDead)
    {
      return;
    }

    animator.Play("Hurt");
    context.StartCoroutine(ExitHurtState());
  }

  private IEnumerator ExitHurtState()
  {
    yield return new WaitForSeconds(hurtDuration);
    isAppliedAnimation = true;
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

    return null;
  }
}
