using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Bullet
{
    [SerializeField]
    private GameObject explosionObj;

    private void ProcessDefaultCollision()
    {
        explosionObj.SetActive(true);
        explosionObj.transform.parent = null;
        explosionObj.transform.position = transform.position;
        var explosion = explosionObj.GetComponent<Explosion>();
        //explosion.DamageAmount = FinalDamage;
    }

    protected  void ProcessObjectCollision()
    {
        explosionObj.SetActive(true);
        explosionObj.transform.parent = null;
        explosionObj.transform.position = transform.position;
        var explosion = explosionObj.GetComponent<Explosion>();
        //explosion.DamageAmount = FinalDamage;
    }
}
