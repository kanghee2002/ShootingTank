using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon, IDefaultgun
{
    [SerializeField] private float maxBulletSize;
    [SerializeField] private float bulletSize;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus,
        float maxChargedDamageBonus)
    {
        GameObject obj = objectPool.GetBullet();
        Vector3 randomDirection = GetRandomDirection(direction, AimAccuracy);
        Bullet bullet = obj.GetComponent<Bullet>();
        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        SetBullet(obj, bullet, randomDirection, damageAmount);

        obj.transform.localScale = Vector2.one * bulletSize;

        Recharge();
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {
        if (bulletSize < maxBulletSize)
        {
            bulletSize += amount;

            if (bulletSize > maxBulletSize)
            {
                bulletSize = maxBulletSize;
            }
        }
    }
}
