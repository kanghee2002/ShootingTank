using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Refrigerator : Boss
{
    [SerializeField] private GameObject[] foodPrefabArray;
    [SerializeField] private GameObject[] iciclePrefabArray;

    [SerializeField][Range(0f, 1f)] private float phaseChangeHealthPercentage = 0.5f;

    [SerializeField] private float attackReadyTime;

    [SerializeField] private Vector3 bulletFirePositionOffset = new Vector3(-5.3f, 0f, 0f);

    [SerializeField] private float bulletFireDistance;

    private Animator animator;

    private int phase = 1;

    private List<string> patternArray = new()
    {
        ThrowFood,
        ThrowIcicle,
    };

    [Header("Attack_ Routine")]
    private const string ThrowFood = "Attack_ThrowFood";
    private const string ThrowIcicle = "Attack_ThrowIcicle";

    [Header("Animation Trigger")]
    private const string openFridgeTrigger = "OpenFridge";
    private const string closeFridgeTrigger = "CloseFridge";
    private const string openFreezerTrigger = "OpenFreezer";
    private const string closeFreezerTrigger = "CloseFreezer";

    private State state = State.Idle;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerTransform = GameObject.Find("Player").transform;
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
            attackCoolTime = 0.25f;
            minThrowPower = 10f;
            maxThrowPower = 20f;

            float elapsedTime = 0f;

            animator.SetTrigger(openFridgeTrigger);
            
            yield return new WaitForSeconds(attackReadyTime);

            while (elapsedTime < attackTime)
            {
                Debug.Log("Throw Random Food");

                GameObject selectedPrefab = foodPrefabArray[Random.Range(0, foodPrefabArray.Length)];

                float randomRadian = Random.Range(-180f, 180f) * Mathf.Deg2Rad;

                float cos = Mathf.Cos(randomRadian);
                float sin = Mathf.Sin(randomRadian);

                float randomX = cos * Vector2.right.x - sin * Vector2.right.y;
                float randomY = sin * Vector2.right.x + cos * Vector2.right.y;

                Vector3 direction = new Vector3(randomX, randomY, 0f);

                GameObject bulletObject = GetBullet(selectedPrefab, transform.position + bulletFirePositionOffset, direction);
                FireBullet(bulletObject, direction, minThrowPower, maxThrowPower, 0f);

                elapsedTime += attackCoolTime;

                yield return new WaitForSeconds(attackCoolTime);
            }

            animator.SetTrigger(closeFridgeTrigger);

            ChangeState(State.Idle);
        }
        else
        {
            // Phase 2
        }
    }

    private IEnumerator Attack_ThrowIcicle()
    {
        float attackTime, attackCoolTime, minThrowPower, maxThrowPower, aimTime;

        if (phase == 1)
        {
            attackTime = 5f;
            attackCoolTime = 0.2f;
            minThrowPower = 20f;
            maxThrowPower = 30f;
            aimTime = 0.1f;

            float elapsedTime = 0f;

            animator.SetTrigger(openFreezerTrigger);

            yield return new WaitForSeconds(attackReadyTime);

            while (elapsedTime < attackTime)
            {
                Debug.Log("Throw Icicle");

                StartCoroutine(ThrowIcicleRoutine(aimTime, minThrowPower, maxThrowPower));
                
                elapsedTime += attackCoolTime;

                yield return new WaitForSeconds(attackCoolTime);
            }

            animator.SetTrigger(closeFreezerTrigger);

            ChangeState(State.Idle);
        }
        else
        {
            // Phase 2
        }
    }

    private IEnumerator ThrowIcicleRoutine(float aimTime, float minThrowPower, float maxThrowPower)
    {
        float positionOffset = 3f;
        float angleOffset = 5f;

        GameObject selectedPrefab = iciclePrefabArray[Random.Range(0, iciclePrefabArray.Length)];

        Vector3 randomPositionOffset = new Vector3(Random.Range(-positionOffset, positionOffset), Random.Range(-positionOffset, positionOffset), 0f);

        Vector3 bulletPosition = transform.position + bulletFirePositionOffset + randomPositionOffset;

        Vector3 direction = (playerTransform.position - bulletPosition).normalized;

        float randomRadian = Random.Range(-angleOffset, angleOffset) * Mathf.Deg2Rad;

        float cos = Mathf.Cos(randomRadian);
        float sin = Mathf.Sin(randomRadian);

        float randomX = cos * direction.x - sin * direction.y;
        float randomY = sin * direction.x + cos * direction.y;

        Vector3 randomDirection = new Vector3(randomX, randomY, 0f);


        GameObject bulletObject = GetBullet(selectedPrefab, bulletPosition, randomDirection);

        yield return new WaitForSeconds(aimTime);

        FireBullet(bulletObject, randomDirection, minThrowPower, maxThrowPower, 0f);
    }

    private void FireBullet(GameObject bulletObject, Vector3 direction, float minThrowPower, float maxThrowPower, float damageValue)
    {
        float bulletSpeed = Random.Range(minThrowPower, maxThrowPower);

        bulletObject.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.LookAtDirection(bulletObject, direction);
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag(Settings.playerTag);
    }

    private GameObject GetBullet(GameObject prefab, Vector3 position, Vector3 direction)
    {
        GameObject bulletObject = objectPool.GetBullet(prefab);
        bulletObject.transform.position = position + direction * bulletFireDistance;

        return bulletObject;
    }
}
