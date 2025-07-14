using System.Collections;
using UnityEngine;

public abstract class BasePlayerAttackState : BasePlayerState
{
    protected float animationDuration = 0.6f;
    protected float comboWindow = 1.5f;
    protected float attackRange = 0.5f;
    protected float damageMultiplier = 1.0f;
    protected float comboTimer = 0f;
    protected bool hasAppliedAnimation = false;
    protected bool hasAppliedDamage = false;
    protected Coroutine animationCoroutine;

    protected GameObject attackPoint;
    protected Animator animator;
    protected string animationName;

    public BasePlayerAttackState(PlayerController context)
        : base(context)
    {
        attackPoint = context.PlayerStats.AttackPoint;
        animator = context.GetComponent<Animator>();
    }

    public override void Enter()
    {
        hasAppliedAnimation = false;
        hasAppliedDamage = false;
        comboTimer = comboWindow;

        animator.Play(animationName);
        animationCoroutine = context.StartCoroutine(WaitForAnimationCompletion());
        PerformDamage();
    }

    public override void Exit()
    {
        if (animationCoroutine != null)
        {
            context.StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    public override void FixedUpdate()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.fixedDeltaTime;
        }
    }

    protected virtual void PerformDamage()
    {
        float baseDamage = context.PlayerStats.AttackDamage;
        float finalDamage = baseDamage * damageMultiplier;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            attackPoint.transform.position,
            attackRange,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D collider in colliders)
        {
            EnemyStats enemyStats = collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(finalDamage);
            }
        }
    }

    protected IEnumerator WaitForAnimationCompletion()
    {
        yield return new WaitForSeconds(animationDuration);
        hasAppliedAnimation = true;
    }

    protected bool CanTransitionToNextAttack() =>
        comboTimer > 0f && context.PlayerInput.IsAttackHeld();

    protected bool IsAnimationComplete() => hasAppliedAnimation;

    protected virtual void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
        }
    }
}
