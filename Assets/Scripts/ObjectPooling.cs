using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private int poolSize;

    [SerializeField]
    private float bulletRotatedDegree;

    private Queue<GameObject> bulletPool;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        bulletPool = new Queue<GameObject>();
        MakeBulletPool();
    }

    private void MakeBulletPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            bulletPool.Enqueue(MakeBullet());
        }
    }

    private GameObject MakeBullet()
    {
        var obj = Instantiate(bulletPrefab);
        obj.GetComponent<Bullet>().objectPool = this;
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }

    public GameObject GetBullet()
    {
        GameObject obj;
        if (bulletPool.Count > 0)
        {
            obj = bulletPool.Dequeue();
        }
        else
        {
            obj = MakeBullet();
        }
        obj.transform.SetParent(null);
        obj.SetActive(true);
        obj.transform.rotation = Quaternion.identity;
        return obj;
    }

    public void ReturnBullet(GameObject obj)
    {
        obj.transform.position = transform.position;
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        bulletPool.Enqueue(obj);
    }

    public void LookAtDirection(GameObject obj, Vector3 direction)
    {
        float rotateDegree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree + bulletRotatedDegree);
    }
}
