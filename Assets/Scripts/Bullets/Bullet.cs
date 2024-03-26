using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public ShootingObject shootingObject;

    [SerializeField]
    private float speed;
    public float Speed { get => speed;}

    private float finalDamage;
    public float FinalDamage { get => finalDamage; set => finalDamage = value; }

    [SerializeField]
    private float lifeTIme;

    [SerializeField]
    private float rotatedDegree;
    public float RotatedDegree { get => rotatedDegree;}

    private List<string> defaultCollisionTags = new();

    protected abstract string TargetTag { get; }

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        defaultCollisionTags.Add("Platform");
    }

    private void OnEnable()
    {
        StartCoroutine(CheckLifeTime(lifeTIme));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var defaultTag in defaultCollisionTags)
        {
            if (collision.collider.CompareTag(defaultTag))
            {
                ProcessDefaultCollision();
                DestroyBullet();
                return;
            }
        }

        if (collision.collider.CompareTag(TargetTag))
        {
            collision.transform.GetComponent<IDamageable>().Damage(FinalDamage);
            DestroyBullet();

        }
    }

    private IEnumerator CheckLifeTime(float time)
    {
        while (time > 0f)
        {
            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        DestroyBullet();
    }

    /// <summary>
    /// Platform, Wall 등과의 충돌
    /// </summary>
    protected virtual void ProcessDefaultCollision()
    {
        //Do Particle, Sound, etc...
    }
    /// <summary>
    /// Player, Enemy 등과의 충돌
    /// </summary>
    /// 
    protected virtual void ProcessObjectCollision()
    {
        //Do Particle, Sound, etc...
    }

    protected void DestroyBullet()
    {
        //StopAllCoroutines();
        if (!shootingObject)
        {
            Debug.Log("Error: No Shooting Object in Bullet");
            Destroy(gameObject);
        }
        else shootingObject.ReturnBullet(gameObject);
    }
}
