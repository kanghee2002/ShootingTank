using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    /// <summary>
    /// 한 번에 발사되는 탄환 수
    /// </summary>
    [SerializeField]
    private int pelletCount;
    public int PelletCount { get => pelletCount; set => pelletCount = value; }

    public override GameObject Fire(Vector3 dir)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return null;
        }

        for (int i = 0; i < pelletCount; i++)
        {
            var randomDir = GetRandomDir(dir, AimAccuracy);
            var obj = base.Fire(randomDir);
            obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        }
        CurAmmo--;
        Recharge();

        return null;
    }
}
