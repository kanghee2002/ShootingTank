using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Refrigerator : Boss
{
    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject[] foodPrefabArray;
    [SerializeField] private GameObject[] iciclePrefabArray;

    [Header("Tackle References")]
    [SerializeField] private WarningLaser warningLaser;
    [SerializeField] private Sprite tackleSprite;
    [SerializeField] private LayerMask tackleBlockingLayer;

    [Header("Default Settings")]
    [SerializeField][Range(0f, 1f)] private float phaseChangeHealthPercentage = 0.5f;
    [SerializeField] private float attackReadyTime;
    [SerializeField] private float bulletFireDistance;

    [Header("Attack Damage")]
    [SerializeField] private float foodDamage;
    [SerializeField] private float icicleDamage;
    [SerializeField] private float tackleDamage;

    [Header("Attack_ Routine")]
    private const string ThrowFood = "Attack_ThrowFood";
    private const string ThrowIcicle = "Attack_ThrowIcicle";
    private const string Tackle = "Attack_Tackle";

    [Header("Animation Trigger")]
    private const string openFridgeTrigger = "OpenFridge";
    private const string closeFridgeTrigger = "CloseFridge";
    private const string openFreezerTrigger = "OpenFreezer";
    private const string closeFreezerTrigger = "CloseFreezer";
    private const string startGroggyTrigger = "StartGroggy";
    private const string endGroggyTrigger = "EndGroggy";

    private readonly Vector3 centerPositionOffset = new Vector3(-5.3f, 0f, 0f);
    private readonly Vector3 foodThrowPositionOffset = new Vector3(0f, -1.5f, 0f);
    private readonly Vector3 icicleThrowPositionOffset = new Vector3(0f, 3.5f, 0f);

    private Animator animator;
    private PolygonCollider2D polygonCollider;

    private Sprite idleSprite;

    private int phase = 1;

    private Dictionary<string, int> patternCountDictionary = new()
    {
        {ThrowFood, 0 },
        {ThrowIcicle, 0},
        {Tackle, 0},
    };

    private State state = State.Idle;

    private bool isTackling = false;
    private bool hasTackled = false;

    private bool hasDonePhaseAction = false;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        idleSprite = spriteRenderer.sprite;

        health.onDie += () => gameObject.SetActive(false);
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerTransform = GameObject.Find("Player").transform;

            phase = 1;
            hasDonePhaseAction = false;

            ChangeState(State.Idle);
            isTackling = false;
            hasTackled = false;


            health.onHealthChanged += OnPhaseChanged;
        }
    }

    public override void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;

        ChangeState(State.Idle);

        phase = 1;

        isTackling = false;
        hasTackled = false;

        hasDonePhaseAction = false;

        health.onHealthChanged += OnPhaseChanged;
    }
    
    private void OnPhaseChanged(float currentHealth, float maxHealth)
    {
        if (phase == 1 && (currentHealth / maxHealth) < phaseChangeHealthPercentage)
        {
            phase = 2;

            idleTime = 1.5f;
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
        Vector2 originPosition;

        int moveCount;

        float moveDistance;

        #region OnPhaseChangeAction
        if (phase == 2 && !hasDonePhaseAction)
        {
            originPosition = transform.position;

            moveCount = 24;

            moveDistance = 5f;
            
            float moveSpeed = 40f;

            for (int count = 0; count < moveCount; count++)
            {
                if (count == moveCount / 6)
                {
                    animator.SetTrigger(startGroggyTrigger);
                }
                else if (count == moveCount - 1)
                {
                    animator.SetTrigger(endGroggyTrigger);
                }

                Vector2 randomVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Vector2 randomPosition = originPosition + randomVector * moveDistance;

                if (count == moveCount - 1)
                {
                    randomPosition = originPosition;
                    moveSpeed /= 2f;
                }

                Vector2 moveDirection = (randomPosition - (Vector2)transform.position).normalized;

                float moveTime = (randomPosition - (Vector2)transform.position).magnitude / moveSpeed;

                rigid.velocity = moveDirection * moveSpeed;

                yield return new WaitForSeconds(moveTime);
            }

            rigid.velocity = Vector2.zero;

            hasDonePhaseAction = true;
        }
        #endregion OnPhaseChangeAction

        originPosition = transform.position;

        moveDistance = 5f;
        moveCount = 6;
        
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
        List<string> minCountPatternList = new();

        int minCount = int.MaxValue;

        foreach (var patternCount in patternCountDictionary)
        {
            if (patternCount.Value < minCount)
            {
                minCount = patternCount.Value;
            }
        }

        foreach (var patternCount in patternCountDictionary)
        {
            if (patternCount.Value == minCount)
            {
                minCountPatternList.Add(patternCount.Key);
            }
        }

        if (minCountPatternList.Count == 0) Debug.LogError("Refrigerator Error: MinCountPatternList is empty");

        string selectedPattern = minCountPatternList[Random.Range(0, minCountPatternList.Count)];

        patternCountDictionary[selectedPattern] += 1;

        StartCoroutine(selectedPattern);
    }

    private IEnumerator Attack_ThrowFood()
    {
        float attackTime, attackCoolTime, minThrowPower, maxThrowPower;

        if (phase == 1)
        {
            attackTime = 5f;
            attackCoolTime = 0.125f;
            minThrowPower = 10f;
            maxThrowPower = 20f;

        }
        else
        {
            attackTime = 6f;
            attackCoolTime = 0.0625f;
            minThrowPower = 15f;
            maxThrowPower = 25f;
        }

        float elapsedTime = 0f;

        animator.SetTrigger(openFridgeTrigger);

        yield return new WaitForSeconds(attackReadyTime);

        while (elapsedTime < attackTime)
        {
            GameObject selectedPrefab = foodPrefabArray[Random.Range(0, foodPrefabArray.Length)];

            float randomAngle = Random.Range(-180f, 180f);

            Vector3 direction = GetRotatedVector(Vector2.right, randomAngle);

            GameObject bulletObject = GetBullet(selectedPrefab, transform.position + centerPositionOffset + foodThrowPositionOffset, direction);
            FireBullet(bulletObject, direction, minThrowPower, maxThrowPower, foodDamage);

            elapsedTime += attackCoolTime;

            yield return new WaitForSeconds(attackCoolTime);
        }

        animator.SetTrigger(closeFridgeTrigger);

        ChangeState(State.Idle);
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

        }
        else
        {
            attackTime = 6f;
            attackCoolTime = 0.1f;
            minThrowPower = 25f;
            maxThrowPower = 35f;
            aimTime = 0.05f;
        }

        float elapsedTime = 0f;

        animator.SetTrigger(openFreezerTrigger);

        yield return new WaitForSeconds(attackReadyTime);

        while (elapsedTime < attackTime)
        {
            StartCoroutine(ThrowIcicleRoutine(aimTime, minThrowPower, maxThrowPower));

            elapsedTime += attackCoolTime;

            yield return new WaitForSeconds(attackCoolTime);
        }

        animator.SetTrigger(closeFreezerTrigger);

        ChangeState(State.Idle);
    }

    private IEnumerator ThrowIcicleRoutine(float aimTime, float minThrowPower, float maxThrowPower)
    {
        float positionOffset = 2f;
        float angleOffset = 5f;

        GameObject selectedPrefab = iciclePrefabArray[Random.Range(0, iciclePrefabArray.Length)];

        Vector3 randomPositionOffset = new Vector3(Random.Range(-positionOffset, positionOffset), Random.Range(-positionOffset, positionOffset), 0f);

        Vector3 bulletPosition = transform.position + centerPositionOffset + icicleThrowPositionOffset + randomPositionOffset;

        Vector3 direction = (playerTransform.position - bulletPosition).normalized;

        float randomRadian = Random.Range(-angleOffset, angleOffset) * Mathf.Deg2Rad;

        float cos = Mathf.Cos(randomRadian);
        float sin = Mathf.Sin(randomRadian);

        float randomX = cos * direction.x - sin * direction.y;
        float randomY = sin * direction.x + cos * direction.y;

        Vector3 randomDirection = new Vector3(randomX, randomY, 0f);


        GameObject bulletObject = GetBullet(selectedPrefab, bulletPosition, randomDirection);

        yield return new WaitForSeconds(aimTime);

        FireBullet(bulletObject, randomDirection, minThrowPower, maxThrowPower, icicleDamage);
    }

    private IEnumerator Attack_Tackle()
    {
        Vector2 originPosition = transform.position;

        int tackleCount;
        float warningLaserWidth = 10f, warningLaserSpeed, tackleSpeed, groggyTime;
        float laserStartingDistance = 3f, minTackleDistance = 15f;

        if (phase == 1)
        {
            tackleCount = 6;
            warningLaserSpeed = 30f;
            tackleSpeed = 50f;
            groggyTime = 3f;
        }
        else
        {
            tackleCount = 10;
            warningLaserSpeed = 45f;
            tackleSpeed = 65f;
            groggyTime = 4f;
        }

        #region Tackle

        polygonCollider.isTrigger = true;
        animator.enabled = false;
        spriteRenderer.sprite = tackleSprite;

        Vector2 lastDirection = Vector2.right;

        for (int count = 0; count < tackleCount; count++)
        {
            yield return new WaitForSeconds(attackReadyTime);

            #region Warning

            Vector2 myPosition = transform.position + centerPositionOffset;

            Vector2 direction = (playerTransform.position - (Vector3)myPosition).normalized;

            myPosition += direction * laserStartingDistance;

            float laserDistance = 100f;

            RaycastHit2D rayHit = Physics2D.Raycast(myPosition, direction, laserDistance, tackleBlockingLayer);

            Vector2 tacklePosition = myPosition + direction * laserDistance;

            if (rayHit)
            {
                tacklePosition = rayHit.point;
            }

            float tackleDistance = (tacklePosition - myPosition).magnitude;

            if (count != 0 && tackleDistance < minTackleDistance)
            {
                myPosition = transform.position + centerPositionOffset;
                direction = -lastDirection;
                myPosition += direction * laserStartingDistance;

                rayHit = Physics2D.Raycast(myPosition, direction, laserDistance, tackleBlockingLayer);

                tacklePosition = myPosition + direction * laserDistance;

                if (rayHit)
                {
                    tacklePosition = rayHit.point;
                }

                tackleDistance = (tacklePosition - myPosition).magnitude;
            }

            warningLaser.SetLaserWidth(warningLaserWidth);

            warningLaser.StartStretch(myPosition, tacklePosition, warningLaserSpeed);

            float warningTime = tackleDistance / warningLaserSpeed;

            yield return new WaitForSeconds(warningTime);

            #endregion Warning

            #region Rush to Player

            isTackling = true;
            hasTackled = false;

            warningLaser.SetLaserWidth(0f);

            rigid.velocity = direction * tackleSpeed;

            yield return new WaitForSeconds(tackleDistance / tackleSpeed);

            rigid.velocity = Vector2.zero;

            transform.position = (Vector3)tacklePosition - centerPositionOffset;

            isTackling = false;

            lastDirection = direction;

            #endregion Rush to Player
        }

        polygonCollider.isTrigger = false;
        spriteRenderer.sprite = idleSprite;
        animator.enabled = true;

        #endregion Tackle

        #region Return To Origin Position

        animator.SetTrigger(startGroggyTrigger);

        Vector2 directionToOrigin = (originPosition - (Vector2)transform.position).normalized;

        float distanceToOrigin = (originPosition - (Vector2)transform.position).magnitude;

        float moveSpeed = distanceToOrigin / groggyTime;

        rigid.velocity = directionToOrigin * moveSpeed;

        yield return new WaitForSeconds(distanceToOrigin / moveSpeed);

        animator.SetTrigger(endGroggyTrigger);

        rigid.velocity = Vector2.zero;

        transform.position = originPosition;

        #endregion Return To Origin Position

        warningLaser.SetLaserWidth(0f);

        ChangeState(State.Idle);
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

    private Vector3 GetRotatedVector(Vector3 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        float randomX = cos * vector.x - sin * vector.y;
        float randomY = sin * vector.x + cos * vector.y;

        Vector3 rotatedVector = new Vector3(randomX, randomY, 0f);

        return rotatedVector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (!isTackling | hasTackled)
            {
                return;
            }

            hasTackled = true;

            if (collision.TryGetComponent(out Health health))
            {
                health.TakeDamage(tackleDamage);
            }

            float horizontalVelocity = Mathf.Abs(rigid.velocity.x);
            float verticalVelocity = Mathf.Abs(rigid.velocity.y);

            Vector2 direction;

            float tacklePower = 0.002f;

            if (horizontalVelocity > verticalVelocity)
            {
                if (rigid.velocity.x > 0) direction = new Vector2(1f, 1f);
                else direction = new Vector2(-1f, 1f);
            }
            else
            {
                float xGap = (playerTransform.position - transform.position).x;

                if (xGap > 0f) direction = new Vector2(1f, 1f);
                else direction = new Vector2(-1f, 1f);
            }

            playerTransform.GetComponent<PlayerController>().KnockBack(direction * tacklePower, 0.5f);
        }
    }
}
