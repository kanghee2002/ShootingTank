using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public abstract class Enemy : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Chase,
        Attack,
        Dead
    };

    protected Rigidbody2D rigid;
    protected Health health;
    protected ObjectPooling objectPool;

    public Transform PlayerTransform { set => playerTransform = value; }

    [Header("Enemy Settings")]
    [SerializeField] protected EnemyRank enemyRank;

    [SerializeField] protected float coolTime;

    [SerializeField] protected float moveSpeed;

    [SerializeField] protected float damageValue;

    [SerializeField] protected LayerMask groundingLayerMask;

    protected Transform playerTransform;

    protected bool isCool;

    protected State state;

    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }


    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        objectPool = GetComponent<ObjectPooling>();

        isCool = false;
        isPlayerDetected = false;
    }

    public virtual void OnPlayerDetected(Transform playerTransform)
    {
        isPlayerDetected = true;
        this.playerTransform = playerTransform;
    }

    public virtual void OnPlayerLost()
    {
        isPlayerDetected = false;
        this.playerTransform = null;
    }

    protected abstract void Attack(Vector3 direction);
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

    protected bool IsAttackPossible() => (isPlayerDetected && !isCool);

    protected Vector3 GetTargetDirection(Transform target)
        => (target.position - transform.position).normalized;

    protected void ChangeState(State state) => this.state = state;
}
