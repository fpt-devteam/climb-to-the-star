using UnityEngine;

public abstract class BaseBossState : IState
{
  protected BossController context;

  public BaseBossState(BossController context)
  {
    this.context = context;
  }

  public virtual void Enter() { }

  public virtual void Update() { }

  public virtual void FixedUpdate() { }

  public virtual void Exit() { }

  public virtual IState CheckTransitions() => null;
}
