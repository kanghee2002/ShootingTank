using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CoreHealth : Health
{
    [SerializeField] Health parentHealth;

    [SerializeField] private float coreDamageMultiplier;

    public float CoreDamageMultiplier { get => coreDamageMultiplier; }

    public override bool TakeDamage(float damageAmount)
    {
       return parentHealth.TakeDamage(damageAmount * coreDamageMultiplier);
    }

    public void MinusCoreDamageMultiplier(float amount)
    {
        coreDamageMultiplier -= amount;

        if (coreDamageMultiplier < 0.5f)
        {
            coreDamageMultiplier = 0.5f;
        }
    }
}
