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

    [Header("Default Settings")]
    [SerializeField][Range(0f, 1f)] private float phaseChangeHealthPercentage = 0.5f;

    [SerializeField] private float attackReadyTime;

    [SerializeField] private Vector3 bulletFirePositionOffset = new Vector3(-5.3f, 0f, 0f);

    [SerializeField] private float bulletFireDistance;

    [Header("Attack Pattenr Settings")]
    [SerializeField] private LayerMask tackleBlockingLayer;

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

    private Animator animator;
    private PolygonCollider2D polygonCollider;

    private Sprite idleSprite;

    private int phase = 1;

    private List<string> patternArray = new()
    {
        //ThrowFood,
        //ThrowIcicle,
        Tackle
    };

    private State state = State.Idle;

    private bool isTackling = false;
    private bool hasTackled = false;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        idleSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerTransform = GameObject.Find("Player").transform;
            phase = 2;
            ChangeState(State.Idle);
            isTackling = false;
            hasTackled = false;
        }
    }

    public void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        ChangeState(State.Idle);
        phase = 1;
        isTackling = false;
        hasTackled = false;

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
            attackCoolTime = 0.125f;
            minThrowPower = 10f;
            maxThrowPower = 20f;

            float elapsedTime = 0f;

            animator.SetTrigger(openFridgeTrigger);
            
            yield return new WaitForSeconds(attackReadyTime);

            while (elapsedTime < attackTime)
            {
                Debug.Log("Throw Random Food");

                GameObject selectedPrefab = foodPrefabArray[Random.Range(0, foodPrefabArray.Length)];

                float randomAngle = Random.Range(-180f, 180f);

                Vector3 direction = GetRotatedVector(Vector2.right, randomAngle);

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

    private IEnumerator Attack_Tackle()
    {
        Vector2 originPosition = transform.position;

        int tackleCount;
        float warningLaserWidth = 7f, warningLaserSpeed, tackleSpeed, groggyTime;

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
            warningLaserSpeed = 50f;
            tackleSpeed = 70f;
            groggyTime = 4f;
        }

        #region Tackle

        animator.enabled = false;
        spriteRenderer.sprite = tackleSprite;

        Debug.Log(spriteRenderer.sprite);

        for (int count = 0; count < tackleCount; count++)
        {
            yield return new WaitForSeconds(attackReadyTime);

            #region Warning

            Vector2 myPosition = transform.position + bulletFirePositionOffset;

            Vector2 direction = (playerTransform.position - (Vector3)myPosition).normalized;

            myPosition += direction * bulletFireDistance;

            float laserDistance = 100f;

            RaycastHit2D rayHit = Physics2D.Raycast(myPosition, direction, laserDistance, tackleBlockingLayer);

            Vector2 tacklePosition = myPosition + direction * laserDistance;

            if (rayHit)
            {
                tacklePosition = rayHit.point;
            }

            warningLaser.SetLaserWidth(warningLaserWidth);

            warningLaser.StartStretch(myPosition, tacklePosition, warningLaserSpeed);

            float tackleDistance = (tacklePosition - myPosition).magnitude;

            float warningTime = tackleDistance / warningLaserSpeed;

            yield return new WaitForSeconds(warningTime);

            #endregion Warning

            #region Rush to Player

            polygonCollider.isTrigger = true;
            isTackling = true;
            hasTackled = false;

            warningLaser.SetLaserWidth(0f);

            rigid.velocity = direction * tackleSpeed;

            yield return new WaitForSeconds(tackleDistance / tackleSpeed);

            rigid.velocity = Vector2.zero;

            transform.position = (Vector3)tacklePosition - bulletFirePositionOffset;

            polygonCollider.isTrigger = false;
            isTackling = false;

            #endregion Rush to Player
        }

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
