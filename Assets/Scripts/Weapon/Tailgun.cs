using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailgun : Weapon, IMultiFiregun
{
    [Header("Big Bullet")]
    [SerializeField] private float maxBigBulletSize;
    [SerializeField] private float bigBulletSize;

    [Header("Small Bullet")]
    [SerializeField] private float maxSmallBulletSize;
    [SerializeField] private float smallBulletSize;

    [Header("Burst")]
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

        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        GameObject obj = objectPool.GetBullet();
        Vector3 randomDirection = GetRandomDirection(direction, AimAccuracy);
        Bullet bullet = obj.GetComponent<Bullet>();

        SetBullet(obj, bullet, randomDirection, damageAmount);

        obj.transform.localScale = Vector3.one * bigBulletSize;

        yield return new WaitForSeconds(burstIntervalTime);

        for (int i = 0; i < pelletCount; i++)
        {
            obj = objectPool.GetBullet();
            randomDirection = GetRandomDirection(direction, AimAccuracy);
            bullet = obj.GetComponent<Bullet>();

            SetBullet(obj, bullet, randomDirection, damageAmount * 0.6f);

            obj.transform.localScale = Vector3.one * smallBulletSize;

            yield return new WaitForSeconds(burstIntervalTime);
        }
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {
        if (bigBulletSize < maxBigBulletSize)
        {
            bigBulletSize += amount;

            if (bigBulletSize > maxBigBulletSize)
            {
                bigBulletSize = maxBigBulletSize;
            }
        }
    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }
}
