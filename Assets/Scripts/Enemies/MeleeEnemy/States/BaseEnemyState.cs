using UnityEngine;

public class BaseEnemyState : MonoBehaviour, IState
{
    protected EnemyController enemyController;

    public BaseEnemyState(EnemyController enemyController)
    {
        this.enemyController = enemyController;
    }

    public virtual void OnEnter() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }

    public virtual void OnExit() { }
}
