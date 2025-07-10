using UnityEngine;

public class BaseSkill : IPlayerSkill
{
    protected float damage;
    protected float staminaCost;
    protected float cooldown;
    protected float currentCooldown;
    protected float attackRange;
    protected Transform attackPoint;

    protected Player player;
    protected Animator animator;
    protected LayerMask enemyLayers;

    public virtual void Initialize(Player player)
    {
        this.player = player;
        this.attackPoint = player.AttackPoint;
        this.animator = player.GetComponent<Animator>();
        this.enemyLayers = LayerMask.GetMask("Enemy");
    }

    public virtual void Execute()
    {
        if (currentCooldown > 0f)
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

        currentCooldown = cooldown;
    }

    public virtual void Update()
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public virtual bool IsExecutable()
    {
        return currentCooldown <= 0f;
    }
}
