using UnityEngine;

public class Skill1 : BaseSkill
{
    public override void Initialize(Player player)
    {
        base.Initialize(player);
        this.damage = 10f;
        this.staminaCost = 5f;
        this.cooldown = 1f;
        this.currentCooldown = 0f;
    }

    public override void Execute()
    {
        if (!IsExecutable())
        {
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            Debug.Log("Attack: " + enemy.name);
        }

        animator.Play("Attack_1");
        player.TakeStamina(staminaCost);
        currentCooldown = cooldown;
    }
}
