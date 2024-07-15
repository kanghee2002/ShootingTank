using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefaultEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float bulletSpeed;

    [SerializeField] private float weaponLength;

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

        objectPool.LookAtDirection(obj, direction);

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag(Settings.playerTag);
        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;

        return true;
    }
}
