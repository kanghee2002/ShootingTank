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
        weapon.ReturnBullet(gameObject, weapon);
    }
}
