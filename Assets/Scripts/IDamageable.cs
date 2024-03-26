using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    float Hp { get; set; }

    void Damage(float damageAmount);

    void Die();
}
