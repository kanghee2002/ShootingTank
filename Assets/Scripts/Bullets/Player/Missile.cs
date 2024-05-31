using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Bullet
{
    [SerializeField]
    private Explosion explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var defaultCollideTag in defaultCollideTags)
        {
            if (collision.CompareTag(defaultCollideTag))
            {
                ProcessCollision();
                DestroyBullet();
            }
        }
    }

    private void ProcessCollision()
    {
        explosion.DamageAmount = FinalDamage;
        explosion.gameObject.SetActive(true);
        explosion.transform.SetParent(null);
    }
}
