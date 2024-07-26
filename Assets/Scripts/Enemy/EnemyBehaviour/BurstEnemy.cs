using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float bulletSpeed;

    [SerializeField] private float weaponLength;

    [Header("Burst Settings")]
    [SerializeField] private int burstCount;

    [SerializeField] private float burstInterval;

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        StartCoroutine(BurstRoutine(GetTargetDirection(playerTransform)));

        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;

        return true;
    }

    private IEnumerator BurstRoutine(Vector3 direction)
    {
        for (int i = 0; i < burstCount; i++)
        {
            var obj = objectPool.GetBullet();
            obj.transform.position = transform.position + direction * weaponLength;
            obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.LookAtDirection(obj, direction);
            bullet.FinalDamage = damageValue;
            bullet.AddTargetTag(Settings.playerTag);

            yield return new WaitForSeconds(burstInterval);
        }

    }
}
