using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private GameObject[] prefabArray;

    [SerializeField] private int poolSize;

    [SerializeField] private float rotatedDegree;

    private Dictionary<int, Queue<GameObject>> poolQueueDictionary;        // < InstanceID, Object >

    public GameObject prefab;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        poolQueueDictionary = new();

        for (int prefabIndex = 0; prefabIndex < prefabArray.Length; prefabIndex++)
        {
            int poolKey = prefabArray[prefabIndex].GetInstanceID();

            Queue<GameObject> poolQueue = new();
            poolQueueDictionary.Add(poolKey, poolQueue);

            MakePoolQueue(poolKey, prefabIndex);
        }
    }

    private void MakePoolQueue(int poolKey, int index)
    {
        for (int i = 0; i < poolSize; i++)
        {
            poolQueueDictionary[poolKey].Enqueue(MakeBullet(index));
        }
    }

    private GameObject MakeBullet(int index)
    {
        var obj = Instantiate(prefabArray[index]);

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.objectPool = this;
        bullet.prefabID = prefabArray[index].GetInstanceID();
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
        int poolKey = prefabArray[0].GetInstanceID();

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

    public void LookAtDirection(GameObject obj, Vector3 direction)
    {
        float rotateDegree = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree + rotatedDegree);
    }
}
