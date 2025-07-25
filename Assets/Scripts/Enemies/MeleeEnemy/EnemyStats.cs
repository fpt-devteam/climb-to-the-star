using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
  [Header("Health Settings")]
  [SerializeField] private float maxHealth = 100f;
  [SerializeField] private float currentHealth = 100f;
  [SerializeField] private float immuneDuration = 0.5f;

  [Header("Combat Stats")]
  [SerializeField] private float attackDamage = 7f;
  [SerializeField] private float moveSpeed = 4f;
  [SerializeField] private float attackRange = 1.5f;
  [SerializeField] private float attackCooldown = 1.5f;

  [Header("Patrol Settings")]
  [SerializeField] private float patrolDistance = 3f;

  [Header("UI References")]
  [SerializeField] private Slider healthBar;
  [SerializeField] private GameObject attackPoint;

  private const float HIGH_HEALTH_THRESHOLD = 0.6f;
  private const float MEDIUM_HEALTH_THRESHOLD = 0.3f;
  private float immuneTimer = 0f;

  private ImmunityBlinker immunityBlinker;

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
  public bool CanAttack => !IsImmune && !IsHurt && IsAlive;

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

    // Get or add the immunity blinker component
    immunityBlinker = GetComponent<ImmunityBlinker>();
    if (immunityBlinker == null)
    {
      immunityBlinker = gameObject.AddComponent<ImmunityBlinker>();
    }
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
      if (immunityBlinker != null && !immunityBlinker.IsBlinking)
      {
        immunityBlinker.StartBlinking();
      }

      immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);

      if (immuneTimer <= 0f && immunityBlinker != null && immunityBlinker.IsBlinking)
      {
        immunityBlinker.StopBlinking();
      }
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
  }

  private void UpdateHealthSlider()
  {
    if (healthBar == null)
      return;

    var percentage = HealthPercentage;
    healthBar.value = percentage;

    Image fillImage = healthBar.fillRect?.GetComponent<Image>();

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
