using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : ShootingObject, IDamageable
{
    [SerializeField]
    private List<LayerMask> viewObstacleMask = new();

    private CircleCollider2D circleCollider;

    private Transform player;

    [SerializeField]
    private float coolTime;

    private bool isCool;
    private bool isPlayerDetected;

    IDamageable damageableInstance;

    private float hp;

    float IDamageable.Hp { get => hp; set => hp = value; }

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsTargetVisible(other.transform) && isPlayerDetected == false)
            {
                isPlayerDetected = true;
                player = other.transform;
            }
        }
    }

    public LayerMask GetObstacleLayerMask()
    {
        LayerMask newLayerMask = viewObstacleMask[0];
        foreach (var layerMask in viewObstacleMask)
        {
            newLayerMask |= layerMask;
        }
        return newLayerMask;
    }

    private bool IsTargetVisible(Transform target)
    {
        var dir = GetTargetDir(target);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir,
                                                circleCollider.radius, GetObstacleLayerMask());
        
        //Debug
        if (rayHit) Debug.DrawLine(transform.position, rayHit.point, Color.yellow);
        else Debug.DrawLine(transform.position, target.position, Color.red);

        if (rayHit) return false;
        else return true;
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
