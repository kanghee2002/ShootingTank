using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponName title { get; protected set; }
    public string description;
    public float damage;
    public float coolTime;
    public int poolSize;
    public float length;


    protected float speed;

    [SerializeField]
    protected GameObject bulletObj;
    protected Queue<GameObject> bulletPool;

    public virtual void Init()
    {
        speed = bulletObj.GetComponent<Bullet>().speed;
        bulletObj.SetActive(false);
        bulletPool = new Queue<GameObject>();
        MakeBulletPool(ref bulletPool, bulletObj, poolSize);
    }

    public virtual void MakeBulletPool(ref Queue<GameObject> bulletPool, GameObject bullet, int size)
    {
        for (int i = 0; i < size; i++)
        {
            bulletPool.Enqueue(MakeBullet());
        }
    }

    private GameObject MakeBullet()
    {
        var obj = Instantiate(bulletObj);
        obj.GetComponent<Bullet>().weapon = this;
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }

    public virtual GameObject GetBullet()
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
        return obj;
    }

    public virtual void ReturnBullet(GameObject obj)
    {
        obj.transform.position = transform.position;
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        bulletPool.Enqueue(obj);
    }

    public abstract void Fire(Vector3 dir);
}
