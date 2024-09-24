using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float maxHealth;
    public float MaxHealth { get => maxHealth; }
    [SerializeField] protected float currentHealth;
    public float CurrentHealth { get => currentHealth; }

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged onHealthChanged;

    public Action onDie;

    private SpriteRenderer spriteRenderer;

    private float blinkTimer;
    private bool isCoreHit;

    private void Start()
    {
        currentHealth = maxHealth;

        onHealthChanged += SetHealthSliderValue;

        spriteRenderer = GetComponent<SpriteRenderer>();

        blinkTimer = 0f;
    }

    private void Update()
    {
        if (blinkTimer > 0f)
        {
            blinkTimer -= Time.deltaTime;
            
            float newRGB = 0.5f;
            if (spriteRenderer != null)
            {
                if (isCoreHit)
                {
                    spriteRenderer.color = new Color(1f, 0f, 0f);
                }
                else
                {
                    spriteRenderer.color = new Color(1f, newRGB, newRGB);
                }
            }
        }

        if (blinkTimer <= 0f)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }
        }
    }

    public void HealByAmount(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        HealthChanged();
    }

    public void HealByPercentage(float percentage)
    {
        currentHealth += maxHealth * percentage;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        HealthChanged();
    }

    public void IncreaseMaxHealthByPercentage(float percentage)
    {
        float increaseAmount = maxHealth * percentage;

        maxHealth += increaseAmount;

        currentHealth += increaseAmount;

        HealthChanged();
    }

    public void SetHealthSlider(Slider slider)
    {
        healthSlider = slider;
    }

    private void SetHealthSliderValue(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    public virtual bool TakeDamage(float damageAmount, bool isCoreHit = false)          // If Die return true
    {
        currentHealth -= damageAmount;

        blinkTimer = 0.25f;
        this.isCoreHit = isCoreHit;

        HealthChanged();

        if (currentHealth <= 0f)
        {
            Die();
            return true;
        }

        return false;
    }

    private void Die()
    {
        onDie?.Invoke();
    }

    private void HealthChanged()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
