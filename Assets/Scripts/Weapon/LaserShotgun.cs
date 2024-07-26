using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotgun : Weapon
{
    public enum ShootingType
    {
        Random,
        Angle
    }

    [SerializeField] private int pelletCount;
    [SerializeField] private ShootingType shootingType;
    [SerializeField] private float shootingAngle;

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
            var obj = objectPool.GetBullet();
            Laser laser = obj.GetComponent<Laser>();

            laser.AddTargetTag(Settings.enemyTag);
            laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

            obj.transform.position = transform.position + dir * weaponLength;

            laser.Activate(dir);
        }

        CurAmmo--;

        Recharge();
    }
}
