using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstEnemy : Enemy
{
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
            GameObject obj = objectPool.GetBullet();
            Bullet bullet = obj.GetComponent<Bullet>();

            SetBullet(obj, bullet, direction, damageValue);

            yield return new WaitForSeconds(burstInterval);
        }

    }
}
