using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float maxHealth;
    private float currentHealth;
    public float GetHealth { get => currentHealth; }

    public delegate void OnHealthChanged();
    public event OnHealthChanged onHpChanged;

    public delegate void OnDie();
    public event OnDie onDie;

    private void Start()
    {
        currentHealth = maxHealth;

        onHpChanged += SetHealthSliderValue;
    }

    public void SetHealthSlider(Slider slider)
    {
        healthSlider = slider;
    }

    private void SetHealthSliderValue()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        onHpChanged?.Invoke();

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
