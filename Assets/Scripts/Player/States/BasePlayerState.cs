using UnityEngine;

public abstract class BasePlayerState : IState
{
    protected PlayerController context;

    public BasePlayerState(PlayerController context)
    {
        this.context = context;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void Exit() { }

    public virtual IState CheckTransitions() => null;
}
