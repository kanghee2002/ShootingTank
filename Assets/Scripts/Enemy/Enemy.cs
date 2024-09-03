using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected ObjectPooling objectPool;
    protected Health health;

    [Header("Enemy Settings")]
    [SerializeField] protected EnemyRank enemyRank;
    [SerializeField] protected Coin coinPrefab;
    [SerializeField] protected int bonusCoins = 0;

    [Header("Attack Settings")]
    [SerializeField] protected float damageValue;
    [SerializeField] protected float coolTime;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected float weaponLength;

    protected bool isCool;
    public int currentLevel { get; set; }

    Dictionary<EnemyRank, CoinRange> coinRangeDictionary;

    protected virtual void Awake()
    {
        objectPool = GetComponent<ObjectPooling>();
        health = GetComponent<Health>();
        isCool = false;

        coinRangeDictionary = new()
        {
            { EnemyRank.C, new CoinRange(5, 10, bonusCoins) },
            { EnemyRank.B, new CoinRange(8, 15, bonusCoins) },
            { EnemyRank.A, new CoinRange(20, 30, bonusCoins) },
            { EnemyRank.S, new CoinRange(50, 10, bonusCoins) },
        };

        health.onDie += DropCoins;
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

    protected void DropCoins()
    {
        int totalCoins = coinRangeDictionary[enemyRank].GetRandomCoins();

        int minCoinValue = 3, maxCoinValue = 5, droppedCoinsValue = 0;

        while (droppedCoinsValue < totalCoins)
        {
            int currentCoinValue;
            if (totalCoins -  droppedCoinsValue > maxCoinValue)
            {
                currentCoinValue = Random.Range(minCoinValue, maxCoinValue + 1);
            }
            else
            {
                currentCoinValue = totalCoins - droppedCoinsValue;
            }

            Coin coinObject = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            float degree = 30f, radian = degree * Mathf.Deg2Rad, randomRadian = Random.Range(-radian, radian);
            Vector3 randomDir = new Vector3(
                    0 * Mathf.Cos(randomRadian) - 1 * Mathf.Sin(randomRadian),
                    0 * Mathf.Sin(randomRadian) + 1 * Mathf.Cos(randomRadian),
                    0);
            Rigidbody2D coinRigid = coinObject.GetComponent<Rigidbody2D>();
            coinRigid.AddForce(randomDir * 300f);

            coinObject.value = currentCoinValue;

            droppedCoinsValue += currentCoinValue;
        }
    }
}

public class CoinRange
{
    private int minCoins;
    private int maxCoins;

    private int bonusCoins;

    public int MinCoins { get => minCoins; }
    public int MaxCoins { get => maxCoins; }
    public int BonusCoins { get => bonusCoins; }

    public CoinRange(int minCoins, int maxCoins, int bonusCoins)
    {
        this.minCoins = minCoins;
        this.maxCoins = maxCoins;
        this.bonusCoins = bonusCoins;
    }

    public int GetRandomCoins() => Random.Range(minCoins, maxCoins + 1) + bonusCoins;
}
