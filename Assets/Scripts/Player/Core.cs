using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float coreDamageMultiplier;

    private IDamageable damageable;


    private void Awake()
    {
        damageable = transform.parent.GetComponent<IDamageable>();
    }

    void IDamageable.Damage(float damageAmount)
    {
        damageable.Damage(damageAmount * coreDamageMultiplier);
        /*Debug.Log("Core Damage! : " + damageAmount * coreDamageMultiplier + " to " +
            damageable.ToString());*/
    }

    #region Useless
    /// <summary>
    /// Core 시스템 상 사용하지 않음
    /// </summary>
    void IDamageable.Die()
    {

    }

    #endregion
}
