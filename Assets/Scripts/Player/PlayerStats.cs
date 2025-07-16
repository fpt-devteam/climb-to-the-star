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

    [SerializeField]
    private float jumpForce = 7f;

    [SerializeField]
    private float moveSpeed = 7f;

    [Header("Movement Stats")]
    [SerializeField]
    private float moveSpeedInAir = 5f;

    [SerializeField]
    private float airMoveSpeed = 3f;

    [SerializeField]
    private float maxFallSpeed = 10f;

    [SerializeField]
    private float dashForce = 40f;

    [Header("UI References")]
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Slider staminaBar;

    [SerializeField]
    private GameObject attackPoint;

    private float immuneTimer = 0f;
    private bool isShielding = false;

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

    public bool IsShielding => isShielding;
    public bool IsAlive => currentHealth > 0f;
    public bool IsImmune => immuneTimer > 0f;
    public bool IsHurt => immuneTimer > 0f;
    public bool IsDead => currentHealth <= 0f;

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
    }

    private void Update()
    {
        UpdateImmuneTimer();
        UpdateUI();
    }

    private void UpdateImmuneTimer()
    {
        if (immuneTimer > 0f)
        {
            immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);
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
        if (immuneTimer > 0f || isShielding || damageAmount <= 0f)
            return;

        DeductHealth(damageAmount);

        immuneTimer = immuneDuration;
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
