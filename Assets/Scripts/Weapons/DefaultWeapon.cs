using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    public override GameObject Fire(Vector3 dir)
    {
        var obj = GetBullet();
        var randomDir = GetRandomDir(dir, AimAccuracy);
        obj.transform.position = transform.position + randomDir * length;
        obj.GetComponent<Rigidbody2D>().velocity = randomDir * speed;
        LookAtObject(obj.transform, randomDir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        Recharge();
        return obj;
    }
}
