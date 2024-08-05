using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Burstgun : Weapon, IMultiFiregun
{
    [SerializeField] private float maxBulletSize;

    [SerializeField] private float bulletSize;

    [SerializeField] private int pelletCount;

    [SerializeField] private float burstTime;

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
        float burstIntervalTime = burstTime / pelletCount;

        for (int i = 0; i < pelletCount; i++)
        {
            var obj = objectPool.GetBullet();

            var randomDirection = GetRandomDirection(direction, AimAccuracy);

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
            bullet.LookAtDirection(obj, randomDirection);
            bullet.AddTargetTag(Settings.enemyTag);

            obj.transform.position = transform.position + randomDirection * weaponLength;
            obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;
            obj.transform.localScale = Vector3.one * bulletSize;

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
        pelletCount += count;
    }
}
