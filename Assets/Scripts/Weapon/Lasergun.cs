using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Weapon, ILasergun
{
    [SerializeField] private float laserWidth;

    [SerializeField] private float laserDuration;

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
        obj.transform.position = transform.position + randomDirection * weaponLength;

        Laser laser = obj.GetComponent<Laser>();
        laser.firedWeapon = this;
        laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
        laser.FinalDamageOnCoreHit = laser.FinalDamage * coreHitDamageMultiplier;
        laser.AddTargetTag(Settings.enemyTag);

        laser.SetDuration(laserDuration);
        laser.SetLaserWidth(laserWidth);

        laser.Activate(randomDirection);

        CurAmmo--;

        Recharge();
    }

    void ILasergun.IncreaseLaserWidth(float amount)
    {
        laserWidth += amount;
    }

    void ILasergun.IncreaseLaserDuration(float amount)
    {
        laserDuration += amount;
    }

}
