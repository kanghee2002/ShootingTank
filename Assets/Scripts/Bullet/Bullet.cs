using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public Weapon weapon;
    public float speed;
    public float damage;

    public void DestroyBullet()
    {
        if (weapon == null) Destroy(gameObject);
        else weapon.ReturnBullet(gameObject, weapon);
    }
}
