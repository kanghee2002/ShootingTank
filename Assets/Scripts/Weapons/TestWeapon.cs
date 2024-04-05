using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : Weapon
{
    public override GameObject Fire(Vector3 dir)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return null;
        }

        var obj = base.Fire(dir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        CurAmmo--;
        Recharge();
        return obj;
    }
}
