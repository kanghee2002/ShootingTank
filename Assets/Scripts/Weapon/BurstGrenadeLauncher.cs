using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstGrenadeLauncher : Weapon, IMultiFiregun, IExplosivegun
{
    [Header("Burst")]
    [SerializeField] private int pelletCount;
    [SerializeField] private float burstTime;

    [Header("Missile")]
    [SerializeField] private float explosionRadius;

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
            ExplosiveBullet bullet = obj.GetComponent<ExplosiveBullet>();

            SetBullet(obj, bullet, randomDirection, damageAmount);

            bullet.explosion.Radius = explosionRadius;

            yield return new WaitForSeconds(burstIntervalTime);
        }
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {

    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }

    void IExplosivegun.IncreaseExplosionRadius(float amount)
    {
        explosionRadius += amount;
    }
}
