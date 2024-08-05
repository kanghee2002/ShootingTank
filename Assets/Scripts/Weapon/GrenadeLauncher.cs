using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : Weapon, IExplosivegun
{
    [SerializeField] private float explosionRadius;

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

        ExplosiveBullet bullet = obj.GetComponent<ExplosiveBullet>();
        bullet.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
        bullet.AddTargetTag(Settings.enemyTag);
        bullet.explosion.Radius = explosionRadius;

        CurAmmo--;

        Recharge();
    }

    void IExplosivegun.IncreaseExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }
}
