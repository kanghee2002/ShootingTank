using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        currentHealth = maxHealth;

        onHealthChanged += SetHealthSliderValue;
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

    public virtual bool TakeDamage(float damageAmount)          // If Die return true
    {
        currentHealth -= damageAmount;

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
