using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{

    protected ObjectPooling objectPool;

    [Header("Enemy Settings")]
    [SerializeField] protected EnemyRank enemyRank;
    [SerializeField] protected float coolTime;
    [SerializeField] protected float damageValue;

    protected bool isCool;

    protected virtual void Awake()
    {
        objectPool = GetComponent<ObjectPooling>();
        isCool = false;
    }

    public abstract void Attack(Transform playerTransform);

    public EnemyRank GetEnemyRank() => this.enemyRank;

    protected virtual IEnumerator CoolDownRoutine(float coolTime)
    {
        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isCool = false;
    }

    protected Vector3 GetTargetDirection(Transform target)
        => (target.position - transform.position).normalized;
}
