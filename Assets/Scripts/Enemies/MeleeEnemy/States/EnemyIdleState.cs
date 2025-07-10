using UnityEngine;

public class EnemyIdleState : BaseEnemyState
{
    public EnemyIdleState(EnemyController enemyController)
        : base(enemyController) { }

    public override void OnEnter()
    {
        animator.Play("Idle");
    }

    public override void FixedUpdate() { }

    public override void OnExit() { }
}
