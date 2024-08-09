using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstLasergun : Weapon, IMultiFiregun, ILasergun
{
    [Header("Burst")]
    [SerializeField] private int pelletCount;
    [SerializeField] private float burstTime;

    [Header("Laser")]
    [SerializeField] private float laserWidth;
    [SerializeField] private float laserDuration;

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
            obj.transform.position = transform.position + randomDirection * weaponLength;

            Laser laser = obj.GetComponent<Laser>();
            laser.firedWeapon = this;
            laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
            laser.FinalDamageOnCoreHit = laser.FinalDamage * coreHitDamageMultiplier;
            laser.AddTargetTag(Settings.enemyTag);

            laser.SetDuration(laserDuration);
            laser.SetLaserWidth(laserWidth);

            laser.Activate(randomDirection);


            yield return new WaitForSeconds(burstIntervalTime);
        }
    }

    void IDefaultgun.IncreaseBulletsize(float amount)
    {

    }

    void ILasergun.IncreaseLaserDuration(float amount)
    {
        laserDuration += amount;
    }

    void ILasergun.IncreaseLaserWidth(float amount)
    {
        laserWidth += amount;
    }

    void IMultiFiregun.IncreasePelletCount(int count)
    {
        pelletCount += count;
    }
}
