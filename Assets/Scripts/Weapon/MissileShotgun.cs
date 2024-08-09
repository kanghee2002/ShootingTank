using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShotgun : Weapon, IMultiFiregun, IExplosivegun
{
    [Header("Shotgun")]
    [SerializeField] private int pelletCount;
    [SerializeField] private Shotgun.ShootingType shootingType;
    [SerializeField] private float shootingAngle;

    [Header("Missile")]
    [SerializeField] private float explosionRadius;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        List<Vector3> directions = new();

        if (shootingType == Shotgun.ShootingType.Random)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                directions.Add(GetRandomDirection(direction, AimAccuracy));
            }
        }
        else if (shootingType == Shotgun.ShootingType.Angle)
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
            ExplosiveBullet bullet = obj.GetComponent<ExplosiveBullet>();
            float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

            SetBullet(obj, bullet, dir, damageAmount);

            bullet.explosion.Radius = explosionRadius;
        }

        CurAmmo--;

        Recharge();
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {

    }

    void IExplosivegun.IncreaseExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }
}
