using System.Collections;
using UnityEngine;

public class BossDeathState : BaseBossState
{
  private Animator animator;
  private bool hasApplyAnimation = false;

  public BossDeathState(BossController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    // AudioManager.Instance.PlaySFX(AudioSFXEnum.BossDeath);
  }

  public override void Enter()
  {
    Debug.Log("Boss entering Death State");
    animator.Play("Death");
    hasApplyAnimation = false;
    context.StartCoroutine(ExitToDefault());
  }

  private IEnumerator ExitToDefault()
  {
    yield return new WaitForSeconds(1f);
    context.gameObject.SetActive(false);
    hasApplyAnimation = true;
    GameManager.Instance.Victory();
  }
}

