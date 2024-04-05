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
        Debug.Log("Core Damage! : " + damageAmount * coreDamageMultiplier + " to " +
            damageable.ToString());
    }

    #region Useless
    //아래의 변수 및 함수는 사용하지 않음
    private float uselessHp;

    /// <summary>
    /// Core 시스템 상 사용하지 않음
    /// </summary>
    float IDamageable.Hp { get => uselessHp; set => uselessHp = value; }
    /// <summary>
    /// Core 시스템 상 사용하지 않음
    /// </summary>
    void IDamageable.Die()
    {

    }

    #endregion
}
