using UnityEngine;

public class BasePlayerState : MonoBehaviour, IState
{
    protected PlayerController playerController;

    public BasePlayerState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void OnExit() { }

    public virtual bool CanSwitchTo(IState state) => true;
}
