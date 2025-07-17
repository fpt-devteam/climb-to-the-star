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

  // Professional blinking system using modular component
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

  // NEW: Professional attack prevention during immunity
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
      Debug.Log($"EnemyStats: Added ImmunityBlinker component to {gameObject.name}");
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
      // Start blinking if we have immunity and aren't already blinking
      if (immunityBlinker != null && !immunityBlinker.IsBlinking)
      {
        immunityBlinker.StartBlinking();
      }

      immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);

      // Stop blinking when immunity ends
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

    // DEAD CELLS: Light screen shake when enemy takes damage
    CameraShake.Shake(0.2f, 0.1f);

    Debug.Log($"Enemy {gameObject.name} took {damageAmount} damage, immunity started for {immuneDuration}s");
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
