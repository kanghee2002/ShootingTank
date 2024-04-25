using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public float aimAccuracy;

    public override GameObject Fire(Vector3 dir)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return null;
        }

        float accuracyRadian = (90 - (aimAccuracy * 9 / 10)) * Mathf.Deg2Rad;
        float randomAngle = Random.Range(-accuracyRadian, accuracyRadian);
        Vector3 randomDir = new Vector3(
                dir.x * Mathf.Cos(randomAngle) - dir.y * Mathf.Sin(randomAngle),
                dir.x * Mathf.Sin(randomAngle) + dir.y * Mathf.Cos(randomAngle),
                0);
        var obj = base.Fire(randomDir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        CurAmmo--;
        Recharge();

        return obj;
    }
}
