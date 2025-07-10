using System.Collections;
using UnityEngine;

public class AttackState : BasePlayerState
{
    private Transform attackPoint;
    private float attackRange = 0.8f;
    private int attackDamage = 15;
    private LayerMask enemyLayers;

    public AttackState(PlayerController playerController, Animator animator)
        : base(playerController, animator)
    {
        this.attackPoint = playerController.AttackPoint;
    }

    public override void OnEnter()
    {
        animator.Play("Attack_1");
        Debug.Log("AttackState");
        Attack();
        playerController.StartCoroutine(ExitToDefault());
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private IEnumerator ExitToDefault()
    {
        yield return new WaitForSeconds(0.4f);
        playerController.stateMachine.SetState(new IdleState(playerController, animator));
    }
}
