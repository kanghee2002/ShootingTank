using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    public override void Init()
    {
        base.Init();
    }

    public override GameObject Fire(Vector3 dir)
    {
        return base.Fire(dir);
    }
}
