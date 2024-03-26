using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    public override GameObject Fire(Vector3 dir)
    {
        var obj = GetBullet();
        obj.transform.position = transform.position + dir * length;
        obj.GetComponent<Rigidbody2D>().velocity = dir * speed;
        LookAtObject(obj.transform, dir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        Recharge();
        return obj;
    }
}
