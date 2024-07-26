using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float bulletSpeed;

    [SerializeField] private float weaponLength;

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

        var obj = objectPool.GetBullet();
        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.LookAtDirection(obj, direction);
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag(Settings.playerTag);

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
