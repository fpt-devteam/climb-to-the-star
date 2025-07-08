using UnityEngine;

public class BasePlayerState : MonoBehaviour, IState
{
    protected PlayerController playerController;
    protected Animator animator;

    public BasePlayerState(PlayerController playerController, Animator animator)
    {
        this.playerController = playerController;
        this.animator = animator;
    }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void OnExit() { }

    public virtual bool CanSwitchTo(IState state) => true;
}
