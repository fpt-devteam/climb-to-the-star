using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField]
    private float maxHealth = 100f;

    [SerializeField]
    private float currentHealth = 100f;

    [SerializeField]
    private float immuneDuration = 0.5f;

    [Header("Combat Stats")]
    [SerializeField]
    private float attackDamage = 7f;

    [SerializeField]
    private float moveSpeed = 4f;

    [SerializeField]
    private float attackRange = 1.5f;

    [SerializeField]
    private float attackCooldown = 1.5f;

    [Header("Patrol Settings")]
    [SerializeField]
    private float patrolDistance = 3f;

    [Header("UI References")]
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private GameObject attackPoint;

    private const float HIGH_HEALTH_THRESHOLD = 0.6f;
    private const float MEDIUM_HEALTH_THRESHOLD = 0.3f;

    private float immuneTimer = 0f;

    public GameObject AttackPoint => attackPoint;
    public float AttackDamage => attackDamage;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public float PatrolDistance => patrolDistance;
    public float PlayerDetectionRange => patrolDistance;

    public bool IsAlive => currentHealth > 0f;
    public bool IsImmune => immuneTimer > 0f;
    public bool IsHurt => immuneTimer > 0f;
    public bool IsDead => currentHealth <= 0f;

    public float HealthPercentage => currentHealth / maxHealth;

    public void Die()
    {
        if (IsDead)
            return;

        currentHealth = 0f;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        immuneTimer = 0f;
    }

    private void Update()
    {
        UpdateImmuneTimer();
        UpdateHealthSlider();
    }

    private void UpdateImmuneTimer()
    {
        if (immuneTimer > 0f)
        {
            immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (immuneTimer > 0f || damageAmount <= 0f)
            return;

        DeductHealth(damageAmount);
        immuneTimer = immuneDuration;
    }

    public void DeductHealth(float damageAmount)
    {
        if (damageAmount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthBar == null)
            return;

        healthBar.value = HealthPercentage;
        UpdateSliderColor(healthBar, HealthPercentage);
    }

    private void UpdateSliderColor(Slider slider, float percentage)
    {
        Image fillImage = slider.fillRect?.GetComponent<Image>();

        if (fillImage == null)
            return;

        if (percentage > HIGH_HEALTH_THRESHOLD)
        {
            fillImage.color = Color.green;
        }
        else if (percentage > MEDIUM_HEALTH_THRESHOLD)
        {
            fillImage.color = Color.yellow;
        }
        else
        {
            fillImage.color = Color.red;
        }
    }
}
