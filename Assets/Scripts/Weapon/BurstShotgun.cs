using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BurstShotgun : Weapon, IMultiFiregun
{
    [SerializeField] private float maxBulletSize;
    [SerializeField] private float bulletSize;

    [Header("Burst")]
    [SerializeField] private int burstPelletCount;
    [SerializeField] private float burstTime;

    [Header("Shotgun")]
    [SerializeField] private int shotgunPelletCount;
    [SerializeField] private Shotgun.ShootingType shootingType;
    [SerializeField] private float shootingAngle;

    private Coroutine currentShootingRoutine;

    public override void Fire(Vector3 direction, float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        currentShootingRoutine = StartCoroutine(BurstFireRoutine(direction, chargeDamageMultiplierBonus, maxChargedDamageBonus));

        CurAmmo--;

        Recharge();
    }

    private IEnumerator BurstFireRoutine(Vector3 direction, float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        float burstIntervalTime = burstTime / burstPelletCount;

        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        for (int i = 0; i < burstPelletCount; i++)
        {



            List<Vector3> directions = new();

            if (shootingType == Shotgun.ShootingType.Random)
            {
                for (int j = 0; j < shotgunPelletCount; j++)
                {
                    directions.Add(GetRandomDirection(direction, AimAccuracy));
                }
            }
            else if (shootingType == Shotgun.ShootingType.Angle)
            {
                if (shotgunPelletCount % 2 == 0)       // Even
                {
                    float defaultAngle = shootingAngle / 2f;
                    for (int j = 0; j < shotgunPelletCount / 2; j++)
                    {
                        directions.Add(GetDirectionByAngle(direction, defaultAngle + shootingAngle * i));
                        directions.Add(GetDirectionByAngle(direction, -(defaultAngle + shootingAngle * i)));
                    }
                }
                else                            // Odd
                {
                    directions.Add(direction);

                    for (int j = 1; j <= shotgunPelletCount / 2; j++)
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
                
                SetBullet(obj, bullet, dir, damageAmount);

                obj.transform.localScale = Vector2.one * bulletSize;
            }

            yield return new WaitForSeconds(burstIntervalTime);
        }
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
        shotgunPelletCount += count;
    }
}
