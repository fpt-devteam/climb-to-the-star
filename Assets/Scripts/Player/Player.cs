using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private List<ISkill> skills = new List<ISkill>();
    [SerializeField]
    private float maxHealth = 100;

    [SerializeField]
    private float currentHealth = 100;

    [SerializeField]
    private float maxStamina = 100;

    [SerializeField]
    private float currentStamina = 100;

    [SerializeField]
    private float damage = 7f;

    [SerializeField]
    private float jumpForce = 7f;

    [SerializeField]
    private float moveSpeed = 7f;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private Slider staminaBar;

    [SerializeField]
    public Transform AttackPoint => attackPoint;

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player died");
    }

    public void Heal(float healthAmount) =>
        currentHealth = Math.Max(maxHealth, currentHealth + healthAmount);

    public void TakeStamina(float staminaAmount) =>
        currentStamina = Math.Max(0f, currentStamina - staminaAmount);

    public void LevelUp()
    {
        Restart();
        // maxHealth += 10f;
        // damage += 2f;
        // jumpForce += 1f;
    }

    public void Restart()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        GenerateStamina();
        UpdateHealthSlider();
        UpdateStaminaSlider();
    }

    private void UpdateHealthSlider()
    {
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        healthBar.value = currentHealth / maxHealth;

        if (fillImage != null)
        {
            if (healthBar.value > 0.6f && healthBar.value <= 1f)
            {
                fillImage.color = Color.green;
            }
            else if (healthBar.value > 0.3f && healthBar.value <= 0.6f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }
    }

    private void GenerateStamina()
    {
        currentStamina += 1f * Time.deltaTime;
        currentStamina = Math.Min(maxStamina, currentStamina);
    }

    private void UpdateStaminaSlider()
    {
        Image fillImage = staminaBar.fillRect.GetComponent<Image>();
        staminaBar.value = currentStamina / maxStamina;

        if (fillImage != null)
        {
            if (staminaBar.value > 0.6f && staminaBar.value <= 1f)
            {
                fillImage.color = Color.green;
            }
            else if (staminaBar.value > 0.3f && staminaBar.value <= 0.6f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }
    }

    public void AddSkill(ISkill skill)
    {
        skill.Initialize(this);
        skills.Add(skill);
    }

    public void ExecuteSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < skills.Count)
        {
            skills[skillIndex].Execute();
        }
    }
}
