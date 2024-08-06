using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefaultEnemy : Enemy
{
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

        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;

        return true;
    }
}
