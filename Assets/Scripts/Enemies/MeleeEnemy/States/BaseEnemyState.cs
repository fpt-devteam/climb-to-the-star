using UnityEngine;

public abstract class BaseEnemyState : IState
{
    protected EnemyController context;

    public BaseEnemyState(EnemyController context)
    {
        this.context = context;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void Exit() { }

    public virtual IState CheckTransitions() => null;
}
