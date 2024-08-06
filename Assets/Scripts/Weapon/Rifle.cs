using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon, IDefaultgun
{
    [SerializeField] private float bulletSize;

    [SerializeField] private float maxBulletSize;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus,
        float maxChargedDamageBonus)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        GameObject obj = objectPool.GetBullet();
        Vector3 randomDirection = GetRandomDirection(direction, AimAccuracy);
        Bullet bullet = obj.GetComponent<Bullet>();
        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        SetBullet(obj, bullet, randomDirection, damageAmount);

        obj.transform.localScale = Vector2.one * bulletSize;

        CurAmmo--;
        
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
