using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasergun : Weapon
{
    public override GameObject Fire(Vector3 dir)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return null;
        }
        var randomDir = GetRandomDir(dir, AimAccuracy);
        var obj = base.Fire(randomDir);
        Laser laser = obj.GetComponent<Laser>();
        laser.AddTargetTag("Enemy");
        laser.FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        laser.Activate(randomDir);
        CurAmmo--;
        Recharge();

        return obj;
    }
}
