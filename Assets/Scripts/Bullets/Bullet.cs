using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public ShootingObject shootingObject { get => shootingObject; set => shootingObject = value; }

    [SerializeField]
    private float speed;
    public float Speed { get => speed;}

    [SerializeField]
    private float damageValue;
    public float DamageValue { get => damageValue;}

    [SerializeField]
    private float lifeTIme;

    [SerializeField]
    private float rotatedDegree;
    public float RotatedDegree { get => rotatedDegree;}

    private List<string> defaultCollisionTags = new List<string>();

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
                return;
            }
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

    protected virtual void ProcessDefaultCollision()
    {
        DestroyBullet();
    }

    protected virtual void Damage<T>(T target) where T : IDamageable
    {

    }

    protected virtual void DestroyBullet()
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
