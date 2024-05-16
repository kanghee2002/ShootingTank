using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Bullet
{
    [SerializeField]
    private GameObject explosionObj;

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
        explosionObj.GetComponent<Explosion>().DamageAmount = FinalDamage;
        explosionObj.SetActive(true);
        explosionObj.transform.parent = null;
    }
}
