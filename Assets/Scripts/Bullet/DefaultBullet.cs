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

                    bool hasKilled = coreHealth.TakeDamage(FinalDamageOnCoreHit);

                    if (firedWeapon)
                    {
                        firedWeapon.onCoreHit?.Invoke(FinalDamageOnCoreHit * coreHealth.CoreDamageMultiplier);

                        if (hasKilled)
                        {
                            firedWeapon.onKill?.Invoke();
                        }
                    }


                    DestroyBullet();
                } 

                else if (collision.TryGetComponent(out Health health))
                {
                    hasDamaged = true;

                    bool hasKilled = health.TakeDamage(FinalDamage);

                    if (firedWeapon)
                    {
                        firedWeapon.onHit?.Invoke(FinalDamage);

                        if (hasKilled)
                        {
                            firedWeapon.onKill?.Invoke();
                        }
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
