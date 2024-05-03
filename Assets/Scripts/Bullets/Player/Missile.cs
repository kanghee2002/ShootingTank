using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : PlayerBullet
{
    [SerializeField]
    private GameObject explosionPrefab;

    protected override void ProcessDefaultCollision()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    protected override void ProcessObjectCollision()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
