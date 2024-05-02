using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
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
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        CurAmmo--;
        Recharge();

        return obj;
    }
}
