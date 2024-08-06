using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (hasDamaged)
            {
                return;
            }

            if (collision.CompareTag(targetTag))
            {
                if (collision.TryGetComponent(out CoreHealth coreHealth))
                {
                    hasDamaged = true;

                    coreHealth.TakeDamage(FinalDamageOnCoreHit);

                    if (firedWeapon)
                    {
                        firedWeapon.onCoreHit?.Invoke(FinalDamageOnCoreHit * coreHealth.CoreDamageMultiplier);
                    }

                    DestroyBullet();
                } 

                else if (collision.TryGetComponent(out Health health))
                {
                    hasDamaged = true;

                    health.TakeDamage(FinalDamage);

                    if (firedWeapon)
                    {
                        firedWeapon.onHit?.Invoke(FinalDamage);
                    }

                    DestroyBullet();
                }
            }
        }

        foreach (var defaultCollideTag in defaultCollideTags)
        {
            if (collision.CompareTag(defaultCollideTag))
            {
                DestroyBullet();
            }
        }
    }
}
