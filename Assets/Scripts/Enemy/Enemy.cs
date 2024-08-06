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

    [Header("Attack Settings")]
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float weaponLength;

    protected bool isCool;

    protected virtual void Awake()
    {
        objectPool = GetComponent<ObjectPooling>();
        isCool = false;
    }

    public abstract bool Attack(Transform playerTransform);

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

    protected void SetBullet(GameObject obj, Bullet bullet, Vector3 direction, float damageAmount)
    {
        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        bullet.LookAtDirection(obj, direction);
        bullet.FinalDamage = damageAmount;
        bullet.FinalDamageOnCoreHit = damageAmount;
        bullet.AddTargetTag(Settings.playerTag);
    }
}
