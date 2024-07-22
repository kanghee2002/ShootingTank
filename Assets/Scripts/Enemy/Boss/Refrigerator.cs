using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Refrigerator : Boss
{
    [SerializeField] private GameObject[] bulletPrefabArray;

    [SerializeField][Range(0f, 1f)] private float phaseChangeHealthPercentage = 0.5f;

    [SerializeField] private float throwFoodReadyTime;

    [SerializeField] private float bulletFireDistance;

    private int phase = 1;

    private List<string> patternArray = new()
    {
        ThrowFood,
        //ThrowIcicle,
    };

    private const string ThrowFood = "Attack_ThrowFood";
    private const string ThrowIcicle = "Attack_ThrowIcicle";

    private State state = State.Idle;

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeState(State.Idle);
        }
    }

    public void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        ChangeState(State.Idle);
        phase = 1;

        health.onHealthChanged += OnPhaseChanged;
    }
    
    private void OnPhaseChanged(float currentHealth, float maxHealth)
    {
        if (phase == 1 && (currentHealth / maxHealth) < phaseChangeHealthPercentage)
        {
            phase = 2;

            // Excute Phase Change Animation
        } 
    }

    private void ChangeState(State state)
    {
        this.state = state;
        OnStateChanged(state);
    }

    private void OnStateChanged(State state)
    {
        switch (state)
        {
            case State.Idle:
                StartCoroutine(IdleRoutine());
                break;
            case State.ReadyToAttack:
                ExcutePattern();
                break;
            case State.Attacking:
                break;
        }
    }

    private IEnumerator IdleRoutine()
    {
        Vector2 originPosition = transform.position;

        float moveDistance = 5f;
        int moveCount = 6;
        
        float elapsedTime = 0f;

        while (elapsedTime < idleTime)
        {
            transform.position = new Vector2(transform.position.x, originPosition.y + moveDistance * Mathf.Sin(elapsedTime * (moveCount / 2) * Mathf.PI / idleTime));

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originPosition;

        ChangeState(State.ReadyToAttack);
    }

    private void ExcutePattern()
    {
        string selectedPattern = patternArray[Random.Range(0, patternArray.Count)];

        StartCoroutine(selectedPattern);
    }

    private IEnumerator Attack_ThrowFood()
    {
        float attackTime, attackCoolTime, minThrowPower, maxThrowPower;

        if (phase == 1)
        {
            attackTime = 5f;
            attackCoolTime = 0.1f;
            minThrowPower = 10f;
            maxThrowPower = 30f;
            float elapsedTime = 0f;

            //yield return new WaitForSeconds(throwFoodReadyTime);

            while (elapsedTime < attackTime)
            {
                Debug.Log("Throw Random Food");

                GameObject selectedPrefab = bulletPrefabArray[Random.Range(0, bulletPrefabArray.Length)];

                float randomRadian = Random.Range(-180f, 180f) * Mathf.Deg2Rad;

                float cos = Mathf.Cos(randomRadian);
                float sin = Mathf.Sin(randomRadian);

                float randomX = cos * Vector2.right.x - sin * Vector2.right.y;
                float randomY = sin * Vector2.right.x + cos * Vector2.right.y;

                Vector3 direction = new Vector3(randomX, randomY, 0f);

                FireBullet(selectedPrefab, direction, minThrowPower, maxThrowPower, 0f);

                elapsedTime += attackCoolTime;

                yield return new WaitForSeconds(attackCoolTime);
            }

            ChangeState(State.Idle);
        }
        else
        {

        }
    }

    private IEnumerator Attack_ThrowIcicle()
    {
        float attackTime = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < attackTime)
        {
            Debug.Log("Throw Icicle");

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        ChangeState(State.Idle);
    }

    private void FireBullet(GameObject prefab, Vector3 direction, float minThrowPower, float maxThrowPower, float damageValue)
    {
        float bulletSpeed = Random.Range(minThrowPower, maxThrowPower);

        GameObject bulletObject = objectPool.GetBullet();
        bulletObject.transform.position = transform.position + direction * bulletFireDistance;
        bulletObject.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.LookAtDirection(bulletObject, direction);
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag(Settings.playerTag);
    }
}
