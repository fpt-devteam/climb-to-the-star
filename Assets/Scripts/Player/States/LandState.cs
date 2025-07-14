using System.Collections;
using UnityEngine;

public class LandState : BasePlayerState
{
    private Animator animator;
    private bool isAppliedAnimation = false;

    public LandState(PlayerController context)
        : base(context)
    {
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        animator.Play("Land");
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerLand);
        context.StartCoroutine(ExitToDefault());
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.5f);
        isAppliedAnimation = true;
    }

    public override IState CheckTransitions()
    {
        if (!isAppliedAnimation)
        {
            return null;
        }

        return context.GetState(PlayerState.Locomotion);
    }
}
