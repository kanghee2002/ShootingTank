using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    public override void Fire(Vector3 direction)
    {
        var obj = objectPool.GetBullet();

        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        
        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;

        objectPool.LookAtDirection(obj, randomDirection);

        Recharge();
    }
}
