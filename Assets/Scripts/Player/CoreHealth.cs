using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CoreHealth : Health
{
    [SerializeField] Health parentHealth;

    [SerializeField] private float coreDamageMultiplier;

    public override void TakeDamage(float damageAmount)
    {
        parentHealth.TakeDamage(damageAmount * coreDamageMultiplier);
    }

    public void AddCoreDamageMultiplier(float amount) => coreDamageMultiplier += amount;

    public void MinusCoreDamageMultiplier(float amount) => coreDamageMultiplier -= amount;

}
