using UnityEngine;
using System;
using UnityEngine.UI;
public class BossStats : MonoBehaviour
{
  [Header("Boss Stats")]
  [SerializeField] private float maxHealth;
  [SerializeField] private float currentHealth;
  [SerializeField] private float moveSpeed;
  [SerializeField] private float meleeAttackRange;
  [SerializeField] private float meleeAttackDamage;
  [SerializeField] private float rangeAttackDamage;
  [SerializeField] private float rangeAttackRange;
  [SerializeField] private float attackCooldown;
  [SerializeField] private float immuneDuration;

  [Header("Components")]
  [SerializeField] private GameObject meleeAttackPoint;
  [SerializeField] private GameObject projectilePrefab;
  [SerializeField] private Slider healthBar;

  [Header("Enrage Settings")]
  [SerializeField] private float enrageThreshold;
  [SerializeField] private float enrageAttackCooldown;
  [SerializeField] private float enrageSpeedMultiplier;

  private const float HIGH_HEALTH_THRESHOLD = 0.6f;
  private const float MEDIUM_HEALTH_THRESHOLD = 0.3f;
  private float immuneTimer = 0f;

  public float MaxHealth => maxHealth;
  public float CurrentHealth => currentHealth;
  public float HealthPercentage => currentHealth / maxHealth;

  public float MeleeAttackDamage => meleeAttackDamage;
  public float RangeAttackDamage => rangeAttackDamage;
  public float MeleeAttackRange => meleeAttackRange;
  public float ImmuneTimer => immuneTimer;
  public float ImmuneDuration => immuneDuration;

  public float MoveSpeed => IsEnraged ? moveSpeed * enrageSpeedMultiplier : moveSpeed;
  public float AttackCooldown => IsEnraged ? enrageAttackCooldown : attackCooldown;

  public bool IsImmune => immuneTimer > 0f;
  public bool IsHurt => immuneTimer > 0f;
  public bool IsEnraged => HealthPercentage <= enrageThreshold;
  public bool IsDead => currentHealth <= 0f;
  public bool IsAlive => currentHealth > 0f;
  public bool CanAttack => !IsImmune && !IsHurt && IsAlive;

  public GameObject MeleeAttackPoint => meleeAttackPoint;
  public GameObject ProjectilePrefab => projectilePrefab;

  private void Awake()
  {
    currentHealth = maxHealth;
    immuneTimer = 0f;
  }
  private void Update()
  {
    UpdateUI();
    UpdateImmuneTimer();
  }

  public void TakeDamage(float damageAmount)
  {
    if (immuneTimer > 0f || damageAmount <= 0f)
      return;

    DeductHealth(damageAmount);
    immuneTimer = immuneDuration;
  }

  private void UpdateUI()
  {
    if (healthBar == null)
      return;

    healthBar.value = HealthPercentage;

    if (HealthPercentage > HIGH_HEALTH_THRESHOLD)
    {
      healthBar.fillRect.GetComponent<Image>().color = Color.green;
    }
    else if (HealthPercentage > MEDIUM_HEALTH_THRESHOLD)
    {
      healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
    }
    else
    {
      healthBar.fillRect.GetComponent<Image>().color = Color.red;
    }
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
  private void UpdateImmuneTimer()
  {
    if (immuneTimer > 0f)
    {
      immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);
    }
  }

  private void Die()
  {
    currentHealth = 0f;
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
