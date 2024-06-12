using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : ShootingObject, IDamageable
{
    protected Rigidbody2D rigid;

    private Transform player;
    public Transform Player { get => player; set => player = value; }

    [Header("Enemy Settings")]
    [SerializeField]
    protected float coolTime;

    private bool isCool;
    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }

    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected float maxHp;

    [SerializeField]
    protected float hp;

    [SerializeField]
    private Slider hpSlider;

    public delegate void OnDie();
    public event OnDie onDie;

    private void Awake()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
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

    protected virtual GameObject Attack(Vector2 dir)
    {
        var obj = base.Fire(dir);
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag("Player");
        StartCoroutine(CheckCoolTime(coolTime));
        isCool = true;
        return obj;
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

    protected abstract IEnumerator IdleMove();

    protected void SetHpSlider()
    {
        hpSlider.value = hp / maxHp;
    }

    void IDamageable.Damage(float damageAmount)
    {
        hp -= damageAmount;

        SetHpSlider();

        if (hp <= 0)
        {
            ((IDamageable)this).Die();
        }
    }

    void IDamageable.Die()
    {
        //Do Die
        onDie?.Invoke();
    }
}
