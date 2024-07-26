using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missilegun : Weapon
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

        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
        bullet.LookAtDirection(obj, randomDirection);
        bullet.AddTargetTag(Settings.enemyTag);

        CurAmmo--;

        Recharge();
    }
}
