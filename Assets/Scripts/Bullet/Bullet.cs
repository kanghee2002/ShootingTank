using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public Weapon weapon;

    [SerializeField]
    private float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    private float damageValue;
    public float DamageValue { get => damageValue; private set => damageValue = value; }

    [SerializeField]
    private float lifeTIme;

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

        //if (collision.collider.CompareTag("Player", "Enemy")) Damage();
    }

    private IEnumerator CheckLifeTime(float time)
    {
        while (time > 0f)
        {
            Debug.Log(time);
            time -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        DestroyBullet();
    }

    protected virtual void ProcessDefaultCollision()
    {
        DestroyBullet();
    }

    protected virtual void Damage(Transform target)
    {

    }

    protected virtual void DestroyBullet()
    {
        //StopAllCoroutines();
        if (!weapon)
        {
            Debug.Log("No weapon in Bullet");
            Destroy(gameObject);
        }
        else weapon.ReturnBullet(gameObject);
    }
}
