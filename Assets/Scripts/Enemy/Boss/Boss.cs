using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(ObjectPooling))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Boss : MonoBehaviour
{
    public enum State
    {
        Idle,
        ReadyToAttack,
        Attacking,
    }

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float idleTime;

    protected Transform playerTransform;

    protected Health health;
    protected ObjectPooling objectPool;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        objectPool = GetComponent<ObjectPooling>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
