using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public GameObject prefab;
        public int poolSize;
    }

    [Header("Bullet")]
    [SerializeField] private Pool[] poolArray;

    private Dictionary<int, Queue<GameObject>> poolQueueDictionary;        // < InstanceID, Object >

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        poolQueueDictionary = new();

        for (int prefabIndex = 0; prefabIndex < poolArray.Length; prefabIndex++)
        {
            int poolKey = poolArray[prefabIndex].prefab.GetInstanceID();

            Queue<GameObject> poolQueue = new();
            poolQueueDictionary.Add(poolKey, poolQueue);

            MakePoolQueue(poolKey, prefabIndex);
        }
    }

    private void MakePoolQueue(int poolKey, int index)
    {
        for (int i = 0; i < poolArray[index].poolSize; i++)
        {
            poolQueueDictionary[poolKey].Enqueue(MakeBullet(index));
        }
    }

    private GameObject MakeBullet(int index)
    {
        var obj = Instantiate(poolArray[index].prefab);

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.objectPool = this;
        bullet.prefabID = poolArray[index].prefab.GetInstanceID();
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }

    private GameObject MakeBullet(GameObject prefab)
    {
        var obj = Instantiate(prefab);
        obj.GetComponent<Bullet>().objectPool = this;
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        return obj;
    }

    public GameObject GetBullet()
    {
        int poolKey = poolArray[0].prefab.GetInstanceID();

        GameObject obj;
        if (poolQueueDictionary[poolKey].Count > 0)
        {
            obj = poolQueueDictionary[poolKey].Dequeue();
        }
        else
        {
            obj = MakeBullet(0);
        }
        obj.transform.SetParent(null);
        obj.SetActive(true);
        obj.transform.rotation = Quaternion.identity;
        return obj;
    }

    public GameObject GetBullet(GameObject prefab)
    {
        int poolKey = prefab.GetInstanceID();

        GameObject obj;
        if (poolQueueDictionary[poolKey].Count > 0)
        {
            obj = poolQueueDictionary[prefab.GetInstanceID()].Dequeue();
        }
        else
        {
            obj = MakeBullet(prefab);
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

        int poolKey = obj.GetComponent<Bullet>().prefabID;
        poolQueueDictionary[poolKey].Enqueue(obj);
    }
}
