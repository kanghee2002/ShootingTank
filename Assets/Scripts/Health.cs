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
    [SerializeField] protected float currentHealth;
    public float GetHealth { get => currentHealth; }

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged onHealthChanged;

    public Action onDie;

    private void Start()
    {
        currentHealth = maxHealth;

        onHealthChanged += SetHealthSliderValue;
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

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        onDie?.Invoke();
    }
}
