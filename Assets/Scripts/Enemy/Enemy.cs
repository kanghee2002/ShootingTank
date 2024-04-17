using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : ShootingObject, IDamageable
{
    private Rigidbody2D rigid;

    private Transform player;
    public Transform Player { get => player; set => player = value; }

    [SerializeField]
    private float coolTime;

    private bool isCool;
    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }

    private IDamageable damageableInstance;

    [SerializeField]
    private float hp;

    float IDamageable.Hp { get => hp; set => hp = value; }

    private void Awake()
    {
        damageableInstance = this;
        rigid = GetComponent<Rigidbody2D>();
        isCool = false;
        isPlayerDetected = false;
    }

    protected bool IsAttackPossible() => (isPlayerDetected && !isCool);

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
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            rigid.velocity = Vector3.zero;
        }
    }
}
