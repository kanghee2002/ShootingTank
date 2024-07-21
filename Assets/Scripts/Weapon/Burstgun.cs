using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class Burstgun : Weapon
{
    [SerializeField] private int burstCount;

    [SerializeField] private float burstIntervalTime;

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
        for (int i = 0; i < burstCount; i++)
        {
            var obj = objectPool.GetBullet();

            var randomDirection = GetRandomDirection(direction, AimAccuracy);

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage, chargeDamageMultiplierBonus, maxChargedDamageBonus);
            bullet.LookAtDirection(obj, randomDirection);

            obj.transform.position = transform.position + randomDirection * weaponLength;
            obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;

            yield return new WaitForSeconds(burstIntervalTime);
        }
    }
}
