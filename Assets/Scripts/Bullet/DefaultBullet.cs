using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBullet : Bullet
{
    public float destroyTime;
    private void OnEnable()
    {
        Invoke("DestroyBullet", destroyTime);
    }
}
