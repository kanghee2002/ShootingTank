using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (collision.CompareTag(targetTag))
            {
                if (collision.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(FinalDamage);
                    DestroyBullet();
                }
            }
            else
            {
                DestroyBullet();
            }
        }
    }
}
