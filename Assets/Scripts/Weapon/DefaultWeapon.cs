using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeapon : Weapon
{
    private void Start()
    {
        base.Init();
    }

    public override void Fire(Vector3 dir)
    {
        var obj = base.GetBullet(ref bulletPool, bulletObj);
        obj.transform.position = transform.position + dir * length;
        obj.GetComponent<Rigidbody2D>().velocity = dir * speed;
    }
}
