using System.Collections;
using UnityEngine;

public class EnemyDieState : BaseEnemyState
{
  [SerializeField]
  private float deathAnimationDuration = 1f;

  private Animator animator;
  private bool hasApplyAnimation = false;

  public EnemyDieState(EnemyController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    Debug.Log("Enemy entering Die State");

    context.EnemyStats.Die();

    animator.Play("Die");

    context.StartCoroutine(DeathSequence());
  }

  private IEnumerator DeathSequence()
  {
    yield return new WaitForSeconds(deathAnimationDuration);
    hasApplyAnimation = true;

    context.gameObject.SetActive(false);
  }

  public override IState CheckTransitions()
  {
    return null;
  }
}
