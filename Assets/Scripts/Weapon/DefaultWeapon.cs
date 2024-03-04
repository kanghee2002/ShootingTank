using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    public override void Init()
    {
        base.Init();
        title = WeaponName.DefaultWeapon;
    }

    public override void Fire(Vector3 dir)
    {
        var obj = GetBullet();
        obj.transform.position = transform.position + dir * length;
        obj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }
}
