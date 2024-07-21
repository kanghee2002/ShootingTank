using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        objectPool = GetComponent<ObjectPooling>();
    }
}
