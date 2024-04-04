using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : ShootingObject, IDamageable
{

    private Transform player;
    public Transform Player { get => player; set => player = value; }

    [SerializeField]
    private float coolTime;

    private bool isCool;
    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }

    IDamageable damageableInstance;

    private float hp;

    float IDamageable.Hp { get => hp; set => hp = value; }

    private void Awake()
    {
        damageableInstance = this;
    }

    private void Start()
    {
        isCool = false;
        isPlayerDetected = false;
    }

    private void Update()
    {
        if (IsAttackPossible()) Attack(player);
    }

    private bool IsAttackPossible() => (isPlayerDetected && !isCool);

    private Vector3 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;

    protected virtual void Attack(Transform target)
    {
        var dir = GetTargetDir(target);
        var obj = base.Fire(dir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue;
        StartCoroutine(CheckCoolTime(coolTime));
        isCool = true;
    }

    protected virtual IEnumerator CheckCoolTime(float coolTime)
    {
        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        isCool = false;
    } 

    protected virtual void LookAtPlayer()
    {

    }

    void IDamageable.Damage(float damageAmount)
    {
        damageableInstance.Hp -= damageAmount;

        if (damageableInstance.Hp <= 0)
        {
            damageableInstance.Die();
        }
    }

    void IDamageable.Die()
    {
        //Do Die
    }
}
