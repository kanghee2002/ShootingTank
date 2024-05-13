using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("For Check")]
    public ShootingObject shootingObject;

    private float finalDamage;
    public float FinalDamage { get => finalDamage; set => finalDamage = value; }

    [Header("Default Settings")]
    [SerializeField]
    private float lifeTIme;

    [SerializeField]
    protected List<string> targetTags;

    [SerializeField]
    protected List<string> defaultCollideTags;

    private void OnEnable()
    {
        StartCoroutine(CheckLifeTime(lifeTIme));
    }

    private IEnumerator CheckLifeTime(float time)
    {
        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        DestroyBullet();
    }

    protected virtual void DestroyBullet()
    {
        //StopAllCoroutines();
        if (!shootingObject)
        {
            Debug.Log("Error: No Shooting Object in Bullet");
            Destroy(gameObject);
        }
        else shootingObject.ReturnBullet(gameObject);
    }
}
