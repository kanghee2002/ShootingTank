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
        GameObject obj = objectPool.GetBullet();
        Vector3 randomDirection = GetRandomDirection(direction, AimAccuracy);
        ExplosiveBullet bullet = obj.GetComponent<ExplosiveBullet>();
        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        SetBullet(obj, bullet, direction, damageAmount);

        bullet.explosion.Radius = explosionRadius;

        CurAmmo--;

        Recharge();
    }

    void IExplosivegun.IncreaseExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }
}
