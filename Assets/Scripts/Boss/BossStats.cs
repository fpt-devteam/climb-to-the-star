using UnityEngine;
using System;
using UnityEngine.UI;
public class BossStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float enrageThreshold = 0.3f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float enrageSpeedMultiplier = 1.5f;
    [SerializeField] private float meleeAttackDamage = 20f;
    [SerializeField] private float rangeAttackDamage = 20f;
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float rangeAttackRange = 8f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float enrageAttackCooldown = 1f;
    [SerializeField] private GameObject meleeAttackPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Slider healthBar;


    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsEnraged => HealthPercentage <= enrageThreshold;
    public bool IsHurt { get; private set; }
    public bool IsDead => currentHealth <= 0f;
    public float MoveSpeed => IsEnraged ? moveSpeed * enrageSpeedMultiplier : moveSpeed;
    public float MeleeAttackDamage => meleeAttackDamage;
    public float RangeAttackDamage => rangeAttackDamage;
    public float MeleeAttackRange => meleeAttackRange;
    public GameObject MeleeAttackPoint => meleeAttackPoint;
    public GameObject ProjectilePrefab => projectilePrefab;

    private void Awake()
    {
        currentHealth = maxHealth;
        IsHurt = false;
    }
    private void Update()
    {
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;
        DeductHealth(damage);
    }

    private void UpdateUI()
    {
        if (healthBar == null)
            return;

        healthBar.value = HealthPercentage;
    }

    private void DeductHealth(float healthAmount)
    {
        if (healthAmount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - healthAmount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        currentHealth = 0f;
        IsHurt = false;
        Debug.Log("Boss has died.");
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeAttackPoint.transform.position, meleeAttackRange);
        }
    }
}
