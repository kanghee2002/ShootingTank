using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Weapon
{
    public override void Fire(Vector3 direction)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        var obj = objectPool.GetBullet();
        Laser laser = obj.GetComponent<Laser>();

        laser.AddTargetTag("Enemy");
        laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);

        var randomDirection = GetRandomDirection(direction, AimAccuracy);
        obj.transform.position = transform.position + randomDirection * weaponLength;

        laser.Activate(randomDirection);

        CurAmmo--;

        Recharge();
    }
}
