using UnityEngine;

public class BaseEnemyState : MonoBehaviour, IState
{
    protected EnemyController enemyController;
    protected Animator animator;
    protected Rigidbody2D rb;

    public BaseEnemyState(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.animator = enemyController.Animator;
        this.rb = enemyController.Rigidbody;
    }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void OnExit() { }

    public virtual bool CanSwitchTo(IState state) => true;
}
