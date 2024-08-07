﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon, IMultiFiregun
{
    public enum ShootingType
    {
        Random,
        Angle
    }

    [SerializeField] private float maxBulletSize;
    [SerializeField] private float bulletSize;
    [SerializeField] private int pelletCount;
    [SerializeField] private ShootingType shootingType;
    [SerializeField] private float shootingAngle;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus,
        float maxChargedDamageBonus)
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

        foreach (Vector3 dir in directions)
        {
            GameObject obj = objectPool.GetBullet();
            Bullet bullet = obj.GetComponent<Bullet>();
            float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

            SetBullet(obj, bullet, dir, damageAmount);

            obj.transform.localScale = Vector2.one * bulletSize;
        }

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

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }
}
