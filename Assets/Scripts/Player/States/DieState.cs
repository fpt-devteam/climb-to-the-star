using System.Collections;
using UnityEngine;

public class DieState : BasePlayerState
{
    private Animator animator;
    private bool hasApplyAnimation = false;

    public DieState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Die");
        hasApplyAnimation = false;
        context.StartCoroutine(ExitToDefault());
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(1f);
        hasApplyAnimation = true;
        Time.timeScale = 0f;
        GameManager.Instance.GameOver();
    }
}
