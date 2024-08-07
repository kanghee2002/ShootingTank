﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotgun : Weapon, IMultiFiregun, ILasergun
{
    public enum ShootingType
    {
        Random,
        Angle
    }

    [Header("Shotgun")]
    [SerializeField] private int pelletCount;
    [SerializeField] private ShootingType shootingType;
    [SerializeField] private float shootingAngle;

    [Header("Laser")]
    [SerializeField] private float laserWidth;
    [SerializeField] private float laserDuration;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        List<Vector3> directions = new();

        if (shootingType == ShootingType.Random)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                directions.Add(GetRandomDirection(direction, AimAccuracy));
            }
        }
        else if (shootingType == ShootingType.Angle)
        {
            if (pelletCount % 2 == 0)       // Even
            {
                float defaultAngle = shootingAngle / 2f;
                for (int i = 0; i < pelletCount / 2; i++)
                {
                    directions.Add(GetDirectionByAngle(direction, defaultAngle + shootingAngle * i));
                    directions.Add(GetDirectionByAngle(direction, -(defaultAngle + shootingAngle * i)));
                }
            }
            else                            // Odd
            {
                directions.Add(direction);

                for (int i = 1; i <= pelletCount / 2; i++)
                {
                    directions.Add(GetDirectionByAngle(direction, shootingAngle * i));
                    directions.Add(GetDirectionByAngle(direction, -shootingAngle * i));
                }
            }
        }

        foreach (var dir in directions)
        {
            GameObject obj = objectPool.GetBullet();
            obj.transform.position = transform.position + dir * weaponLength;

            Laser laser = obj.GetComponent<Laser>();

            laser.firedWeapon = this;
            laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
            laser.FinalDamageOnCoreHit = laser.FinalDamage * coreHitDamageMultiplier;
            laser.AddTargetTag(Settings.enemyTag);

            laser.SetDuration(laserDuration);
            laser.SetLaserWidth(laserWidth);

            laser.Activate(dir);
        }

        CurAmmo--;

        Recharge();
    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {

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
