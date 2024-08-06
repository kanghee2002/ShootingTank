using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleEnemy : Enemy
{
    [Header("Rifle Settings")]
    [SerializeField] private int pelletCount;

    [SerializeField] private float fireInterval;

    private int count = 0;

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }


        Vector3 direction = GetTargetDirection(playerTransform);

        GameObject obj = objectPool.GetBullet();
        Bullet bullet = obj.GetComponent<Bullet>();

        SetBullet(obj, bullet, direction, damageValue);

        count++;

        isCool = true;

        if (count == pelletCount)
        {
            count = 0;
            StartCoroutine(CoolDownRoutine(coolTime));
        }
        else
        {
            StartCoroutine(CoolDownRoutine(fireInterval));
        }

        return true;
    }
}
