using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : Bullet
{
    public Explosion explosion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (collision.CompareTag(targetTag))
            {
                ProcessCollision();
                DestroyBullet();
            }
        }

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
        foreach (var targetTag in targetTags)
        {
            explosion.AddTargetLayerMask(targetTag);
        }
        explosion.DamageAmount = FinalDamage;
        explosion.CoreHitDamageAmount = FinalDamage * CoreHitDamageMultiplier;
        explosion.gameObject.SetActive(true);
        explosion.transform.SetParent(null);
    }
}
