using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected Health health;
    protected ObjectPooling objectPool;

    private Transform player;
    public Transform Player { get => player; set => player = value; }

    [Header("Enemy Settings")]
    [SerializeField] protected EnemyRank enemyRank;

    public EnemyRank GetEnemyRank() => this.enemyRank;

    [SerializeField] protected float coolTime;

    protected bool isCool;

    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }

    [SerializeField] protected float moveSpeed;

    [SerializeField] protected float damageValue;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        objectPool = GetComponent<ObjectPooling>();
        isCool = false;
        isPlayerDetected = false;
    }

    protected bool IsAttackPossible() => (isPlayerDetected && !isCool);

    public virtual void OnPlayerDetected(Transform player)
    {
        IsPlayerDetected = true;
        Player = player;
    }

    protected Vector3 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;

    protected abstract void Attack(Vector3 direction);

    protected virtual IEnumerator CheckCoolTime(float coolTime)
    {
        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isCool = false;
    } 

    protected void LookAtPlayer(GameObject headObj, SpriteRenderer spriteRenderer)
    {
        Vector3 targetPos = player.position;
        Vector3 headPos = headObj.transform.position;

        float dy = targetPos.y - headPos.y;
        float dx = targetPos.x - headPos.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg + 180f;

        if ((rotateDegree > 30f && rotateDegree < 150f) || 
            (rotateDegree > 210f && rotateDegree < 330f))
        {
            return;
        }

        headObj.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

        if (Mathf.Abs(rotateDegree - 180f) < 90f)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    protected abstract IEnumerator IdleMove();
}
