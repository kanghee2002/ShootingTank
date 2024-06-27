using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missilegun : Weapon
{
    public override void Fire(Vector3 direction)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        var obj = objectPool.GetBullet();

        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;

        objectPool.LookAtDirection(obj, randomDirection);

        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        
        CurAmmo--;

        Recharge();
    }
}
