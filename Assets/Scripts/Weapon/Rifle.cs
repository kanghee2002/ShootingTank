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

        var obj = objectPool.GetBullet();

        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;
        obj.transform.localScale = Vector2.one * bulletSize;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
        bullet.LookAtDirection(obj, randomDirection);
        bullet.AddTargetTag(Settings.enemyTag);

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
