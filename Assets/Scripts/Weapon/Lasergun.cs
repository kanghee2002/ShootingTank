using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Weapon
{
    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus,
        float maxChargedDamageBonus)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        var obj = objectPool.GetBullet();
        Laser laser = obj.GetComponent<Laser>();

        laser.AddTargetTag(Settings.enemyTag);
        laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;

        laser.Activate(randomDirection);

        CurAmmo--;

        Recharge();
    }
}
