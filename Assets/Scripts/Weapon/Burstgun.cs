﻿using System.Collections;
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

        float damageAmount = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);

        for (int i = 0; i < pelletCount; i++)
        {
            GameObject obj = objectPool.GetBullet();
            Vector3 randomDirection = GetRandomDirection(direction, AimAccuracy);
            Bullet bullet = obj.GetComponent<Bullet>();

            SetBullet(obj, bullet, randomDirection, damageAmount);
            
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
