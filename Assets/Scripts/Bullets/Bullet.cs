using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("For Check")]
    public ObjectPooling objectPool;

    private float finalDamage;
    public float FinalDamage { get => finalDamage; set => finalDamage = value; }

    [Header("Default Settings")]
    [SerializeField]
    protected float lifeTIme;

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
        if (!objectPool)
        {
            Debug.Log("Error: No Shooting Object in" + name + " Bullet");
            Destroy(gameObject);
        }
        else objectPool.ReturnBullet(gameObject);
    }

    public bool AddTargetTag(string tag)
    {
        if (targetTags.Contains(tag))
        {
            return false;
        }
        else
        {
            targetTags.Add(tag);
            return true;
        }
    }
}
