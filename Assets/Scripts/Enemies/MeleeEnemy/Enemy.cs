using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;

    [SerializeField]
    private float currentHealth = 100;

    [SerializeField]
    private float damage = 7f;

    [SerializeField]
    private float moveSpeed = 7f;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private float immuneTimer = 0f;

    [SerializeField]
    private float immuneDuration = 0.5f;

    public void Die()
    {
        Debug.Log("Player died");
    }

    public float HealthPercentage() => currentHealth / maxHealth;

    public float CurrentHealth() => currentHealth;

    public bool IsHurt() => immuneTimer > 0f;

    public bool IsDead() => currentHealth <= 0f;

    void Awake()
    {
        currentHealth = maxHealth;
        immuneTimer = 0f;
    }

    void Update()
    {
        immuneTimer = Mathf.Max(0f, immuneTimer - Time.deltaTime);
        UpdateHealthSlider();
    }

    public void RestoreHealth(float healthAmount)
    {
        currentHealth = Math.Min(maxHealth, currentHealth + healthAmount);
    }

    public void DeductHealth(float damageAmount)
    {
        if (immuneTimer > 0)
            return;

        currentHealth -= damageAmount;
        immuneTimer = immuneDuration;

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthSlider()
    {
        Image fillImage = healthBar.fillRect.GetComponent<Image>();
        healthBar.value = HealthPercentage();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DeductHealth(10f);
        }
    }
}
