using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailgun : Weapon, IMultiFiregun
{
    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        throw new System.NotImplementedException();
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {
        throw new System.NotImplementedException();
    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        throw new System.NotImplementedException();
    }
}
