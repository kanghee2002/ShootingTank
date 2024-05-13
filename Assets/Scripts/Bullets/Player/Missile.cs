using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : PlayerBullet
{
    [SerializeField]
    private GameObject explosionObj;

    protected override void ProcessDefaultCollision()
    {
        explosionObj.SetActive(true);
        explosionObj.transform.parent = null;
        explosionObj.transform.position = transform.position;
        var explosion = explosionObj.GetComponent<Explosion>();
        //explosion.DamageAmount = FinalDamage;
        explosion.StartCoroutine(explosion.Explode());
    }

    protected override void ProcessObjectCollision()
    {
        explosionObj.SetActive(true);
        explosionObj.transform.parent = null;
        explosionObj.transform.position = transform.position;
        var explosion = explosionObj.GetComponent<Explosion>();
        //explosion.DamageAmount = FinalDamage;
        explosion.StartCoroutine(explosion.Explode());
    }
}
