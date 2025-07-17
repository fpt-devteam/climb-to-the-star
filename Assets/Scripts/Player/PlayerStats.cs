using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
  [Header("Health Settings")]
  [SerializeField]
  private float maxHealth = 100f;

  [SerializeField]
  private float currentHealth = 80f;

  [SerializeField]
  private float immuneDuration = 0.5f;

  [Header("Stamina Settings")]
  [SerializeField]
  private float maxStamina = 100f;

  [SerializeField]
  private float currentStamina = 80f;

  [SerializeField]
  private float chargeStaminaRestoreRate = 1f;

  [Header("Combat Stats")]
  [SerializeField]
  private float attackDamage = 7f;

  [SerializeField]
  private float attackRange = 0.5f;

  [SerializeField]
  private float comboWindow = 1.5f;

  [SerializeField]
  private float animationDuration = 0.6f;

  [Header("Dead Cells/Celeste Style Movement")]
  [SerializeField]
  private float jumpForce = 12.5f; // Reduced from 25f to half as requested by user

  [SerializeField]
  private float moveSpeed = 9f; // Faster for responsive feel

  [SerializeField]
  private float moveSpeedInAir = 7f; // Good air control

  [SerializeField]
  private float airMoveSpeed = 5f; // For compatibility

  [SerializeField]
  private float maxFallSpeed = 18f; // Higher for snappier falling

  [SerializeField]
  private float dashForce = 35f; // Reduced from 45f for better jump+dash balance

  [Header("Physics Tuning - Dead Cells Feel")]
  [SerializeField, Range(0.1f, 5f)]
  private float gravityScale = 2.5f; // Reduced further to give more air time for double jump

  [SerializeField, Range(0.05f, 0.25f)]
  private float coyoteTime = 0.12f; // Slightly shorter for tighter feel

  [SerializeField, Range(0.05f, 0.15f)]
  private float jumpBufferTime = 0.08f; // Shorter buffer for precision

  [SerializeField, Range(2f, 5f)]
  private float fallGravityMultiplier = 3.5f; // Higher for Dead Cells feel

  [SerializeField, Range(1.5f, 4f)]
  private float lowJumpMultiplier = 2.8f; // More responsive variable jump

  [Header("UI References")]
  [SerializeField]
  private Slider healthBar;

  [SerializeField]
  private Slider staminaBar;

  [SerializeField]
  private GameObject attackPoint;

  private float immuneTimer = 0f;
  private bool isShielding = false;

  // DEAD CELLS: Dash invincibility system
  private float dashInvincibilityTimer = 0f;
  private bool isDashInvincible = false;

  // Professional blinking system using modular component
  private ImmunityBlinker immunityBlinker;

  public GameObject AttackPoint => attackPoint;
  public float AttackDamage => attackDamage;
  public float AttackRange => attackRange;
  public float ComboWindow => comboWindow;
  public float AnimationDuration => animationDuration;
  public float MaxHealth => maxHealth;
  public float MaxStamina => maxStamina;
  public float CurrentHealth => currentHealth;
  public float CurrentStamina => currentStamina;
  public float JumpForce => jumpForce;
  public float MoveSpeed => moveSpeed;
  public float MoveSpeedInAir => moveSpeedInAir;
  public float AirMoveSpeed => airMoveSpeed;
  public float MaxFallSpeed => maxFallSpeed;
  public float DashForce => dashForce;

  // Physics properties
  public float GravityScale => gravityScale;
  public float CoyoteTime => coyoteTime;
  public float JumpBufferTime => jumpBufferTime;
  public float FallGravityMultiplier => fallGravityMultiplier;
  public float LowJumpMultiplier => lowJumpMultiplier;

  public bool IsShielding => isShielding;
  public bool IsAlive => currentHealth > 0f;
  public bool IsImmune => immuneTimer > 0f;
  public bool IsHurt => immuneTimer > 0f;
  public bool IsDead => currentHealth <= 0f;

  // DEAD CELLS: Dash invincibility properties
  public bool IsDashInvincible => isDashInvincible;
  public bool IsInvincible => IsImmune || IsDashInvincible; // Combined invincibility check

  public float HealthPercentage => currentHealth / maxHealth;
  public float StaminaPercentage => currentStamina / maxStamina;

  public void Die()
  {
    if (IsDead)
      return;

    currentHealth = 0f;
  }

  private void Awake()
  {
    immuneTimer = 0f;
    isShielding = false;

    // Get or add the immunity blinker component
    immunityBlinker = GetComponent<ImmunityBlinker>();
    if (immunityBlinker == null)
    {
      immunityBlinker = gameObject.AddComponent<ImmunityBlinker>();
      Debug.Log("PlayerStats: Added ImmunityBlinker component to player");
    }
  }

  private void Update()
  {
    UpdateImmuneTimer();
    UpdateDashInvincibility(); // DEAD CELLS: Update dash invincibility
    UpdateUI();
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

  // DEAD CELLS: Update dash invincibility timer
  private void UpdateDashInvincibility()
  {
    if (dashInvincibilityTimer > 0f)
    {
      dashInvincibilityTimer = Mathf.Max(0f, dashInvincibilityTimer - Time.deltaTime);

      if (dashInvincibilityTimer <= 0f)
      {
        isDashInvincible = false;
        Debug.Log("Dash invincibility ended");
      }
    }
  }

  private void UpdateUI()
  {
    if (healthBar != null)
      UpdateHealthSlider();
    if (staminaBar != null)
      UpdateStaminaSlider();
  }

  public void StartShield() => isShielding = true;

  public void StopShield() => isShielding = false;

  public void ChargeStamina() => RestoreStamina(chargeStaminaRestoreRate * Time.deltaTime);

  public void RestoreStamina(float staminaAmount)
  {
    if (staminaAmount <= 0f)
      return;

    currentStamina = Mathf.Min(maxStamina, currentStamina + staminaAmount);
  }

  public void DeductStamina(float staminaAmount)
  {
    if (staminaAmount <= 0f)
      return;

    currentStamina = Mathf.Max(0f, currentStamina - staminaAmount);
  }

  public void RestoreHealth(float healthAmount)
  {
    if (healthAmount <= 0f)
      return;

    currentHealth = Mathf.Min(maxHealth, currentHealth + healthAmount);
  }

  public void DeductHealth(float healthAmount)
  {
    if (healthAmount <= 0f)
      return;

    currentHealth = Mathf.Max(0f, currentHealth - healthAmount);

    if (currentHealth <= 0f)
    {
      Die();
    }
  }

  public void TakeDamage(float damageAmount)
  {
    // DEAD CELLS: Check both regular immunity and dash invincibility
    if (immuneTimer > 0f || isDashInvincible || isShielding || damageAmount <= 0f)
      return;

    DeductHealth(damageAmount);

    immuneTimer = immuneDuration;

    // DEAD CELLS: Camera shake when player takes damage
    CameraShake.PlayerHurt();

    Debug.Log($"Player took {damageAmount} damage, immunity started for {immuneDuration}s");
  }

  // DEAD CELLS: Set dash invincibility
  public void SetDashInvincibility(float duration)
  {
    dashInvincibilityTimer = duration;
    isDashInvincible = true;
    Debug.Log($"Dash invincibility activated for {duration}s");
  }

  private void UpdateHealthSlider()
  {
    if (healthBar == null)
      return;

    healthBar.value = HealthPercentage;
  }

  private void UpdateStaminaSlider()
  {
    if (staminaBar == null)
      return;

    staminaBar.value = StaminaPercentage;
  }
}
