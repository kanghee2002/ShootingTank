using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("For Check")]
    public ObjectPooling objectPool;

    public int prefabID;

    private float finalDamage;
    public float FinalDamage { get => finalDamage; set => finalDamage = value; }

    [Header("Default Settings")]
    [SerializeField] protected float lifeTIme;

    [SerializeField] protected float rotatedDegree;

    [SerializeField] protected List<string> targetTags;

    [SerializeField] protected List<string> defaultCollideTags;

    private void OnEnable()
    {
        StartCoroutine(CheckLifeTime(lifeTIme));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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
        if (!objectPool)
        {
            Debug.Log("Error: No ObjectPool in" + name + " Bullet");
            Destroy(gameObject);
        }
        else objectPool.ReturnBullet(gameObject);
    }

    public void LookAtDirection(GameObject obj, Vector3 direction)
    {
        float rotateDegree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree + rotatedDegree);
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
