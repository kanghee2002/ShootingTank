using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class CoreHealth : Health
{
    [SerializeField] Health parentHealth;

    [SerializeField] private float coreDamageMultiplier;
    public float CoreDamageMultiplier { get => coreDamageMultiplier; set => coreDamageMultiplier = value;  }

    public override void TakeDamage(float damageAmount)
    {
        parentHealth.TakeDamage(damageAmount * coreDamageMultiplier);
    }

}
