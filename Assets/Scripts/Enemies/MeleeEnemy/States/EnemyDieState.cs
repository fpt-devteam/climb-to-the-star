using System.Collections;
using UnityEngine;

public class EnemyDieState : BaseEnemyState
{
  private float deathAnimationDuration = 1f;

  private Animator animator;

  public EnemyDieState(EnemyController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    animator.Play("Die");
    Debug.Log("Enemy entering Die State");
    context.StartCoroutine(DeathSequence());
  }

  private IEnumerator DeathSequence()
  {
    yield return new WaitForSeconds(deathAnimationDuration);
    context.gameObject.SetActive(false);
  }
}
