using UnityEngine;

public class EnemyIdleState : BaseEnemyState
{
    public EnemyIdleState(EnemyController enemyController)
        : base(enemyController) { }

    public override void OnEnter()
    {
        animator.Play("Idle");
        Debug.Log("Enemy: Entered Idle State");
    }

    public override void FixedUpdate()
    {
    }

    public override void OnExit()
    {
        Debug.Log("Enemy: Exited Idle State");
    }
}
