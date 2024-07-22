using System;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Dead
    };

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField][Range(0, 1)] private float idleStopProbability = 0.25f;
    [SerializeField] private LayerMask blockingLayerMask;

    [Header("Jump Settings")]
    [SerializeField] private bool canJump = false;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private bool isJumpAtDeadEnd = false;

    [Header("Fly Settings")]
    [SerializeField] private bool canFly = false;

    [Header("Aggression Level Settings")]
    [SerializeField] private int aggressionLevel = 1;
    [SerializeField] private float maintainDistance = 0f;           //Only Used on Aggression Level 2

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 100f;
    [SerializeField] private float stationaryTimeOnAttack = 0f;

    [Header("Detect On Hit Settings")]
    [SerializeField] private Sprite detectSprite;
    [SerializeField] private float detectRadius = 10f;
    [SerializeField] private bool detectOnHit = true;
    [SerializeField] private float detectionRadiusMultiplier = 2.5f;
    [SerializeField] private float moveSpeedOnDetectMultiplier = 1f;

    [Header("Ray Settings")]
    [SerializeField] private float downRayHorizontalOffset = 0.7f;
    [SerializeField] private float downRayLength = 1.5f;
    [Space(5)]
    [SerializeField] private float frontRayVerticalOffset = -0.5f;
    [SerializeField] private float frontRayLength = 1.5f;
    [Space(5)]
    [SerializeField] private float jumpFrontRayLength = 2f;

    private Health health;
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Enemy enemy;
    private Collider2D myCollider;
    private JumpChecker jumpChecker;
    private PlayerDetector playerDetector;

    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; }

    public float DetectRadius { get => detectRadius; }


    public delegate void OnPlayerDetect(Transform playerTransform);
    public OnPlayerDetect onPlayerDetect;
    public delegate void OnPlayerLost();
    public OnPlayerLost onPlayerLost;

    private Transform playerTransform;
    private State state;
    private Coroutine currentMoveRoutine;
    private Sprite idleSprite;

    private void Awake()
    {
        health = GetComponent<Health>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemy = GetComponent<Enemy>();
        myCollider = GetComponent<Collider2D>();

        playerDetector = GetComponentInChildren<PlayerDetector>();
        jumpChecker = GetComponentInChildren<JumpChecker>();
    }

    private void Start()
    {
        idleSprite = spriteRenderer.sprite;

        playerDetector.circleCollider.radius = detectRadius;

        health.onHealthChanged += (float currentHealth, float maxHealth) => playerDetector.ExpandDetectRadius(detectionRadiusMultiplier);

        health.onDie += () => gameObject.SetActive(false);

        onPlayerDetect += EnemyEvent_OnPlayerDetect;

        onPlayerLost += EnemyEvent_OnPlayerLost;

        isPlayerDetected = false;

        currentMoveRoutine = StartCoroutine(IdleMoveRoutine());
    }

    private void Update()
    {
        ExcuteStateAction();
    }

    public void EnemyEvent_OnPlayerDetect(Transform playerTransform)
    {
        if (state != State.Idle)
        {
            return;
        }

        playerDetector.ExpandDetectRadius(detectionRadiusMultiplier);

        isPlayerDetected = true;
        this.playerTransform = playerTransform;

        ChangeState(State.Attack);

        if (detectSprite != null)
        {
            spriteRenderer.sprite = detectSprite;
        }

        StopCoroutine(currentMoveRoutine);
        currentMoveRoutine = StartCoroutine(AttackMoveRoutine());
    }

    public void EnemyEvent_OnPlayerLost()
    {
        if (state != State.Attack)
        {
            return;
        }

        if (!gameObject.activeSelf)
        {
            return;
        }

        playerDetector.ReduceDetectRadius();

        isPlayerDetected = false;
        this.playerTransform = null;

        ChangeState(State.Idle);

        spriteRenderer.sprite = idleSprite;

        StopCoroutine(currentMoveRoutine);
        currentMoveRoutine = StartCoroutine(IdleMoveRoutine());
    }

    private void ExcuteStateAction()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Attack:
                if (isPlayerDetected && Vector2.Distance(playerTransform.position, transform.position) <= attackRange)
                {
                    bool isAttack = enemy.Attack(playerTransform);

                    if (isAttack && stationaryTimeOnAttack > 0f)
                    {
                        PauseMove(stationaryTimeOnAttack);
                    }
                }
                break;
            case State.Chase:
            case State.Dead:
                break;
        }
    }

    private IEnumerator IdleMoveRoutine()
    {
        #region IdleMove

        while (true)
        {
            float moveTime = 1f;

            int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

            SetObjectDirection(moveDirection);

            float stopProbability = Random.Range(0f, 1f);

            while (moveTime > 0f)
            {
                if (stopProbability <= idleStopProbability)
                {
                    rigid.velocity = new Vector2(0f, rigid.velocity.y);
                }
                else
                {
                    rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                    #region Platform Check

                    bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                    bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                    //Stop in front of Cliff
                    if (canJump)
                    {
                        bool isFrontHighCliff = CheckFrontCliff(moveDirection, jumpHeight + 0.3f, false);

                        if (isFrontCliff && isFrontHighCliff)
                        {
                            rigid.velocity = new Vector2(0, rigid.velocity.y);
                            yield return new WaitForFixedUpdate();
                            break;
                        }
                    }
                    else
                    {
                        if (isFrontCliff)
                        {
                            rigid.velocity = new Vector2(0, rigid.velocity.y);
                            yield return new WaitForFixedUpdate();
                            break;
                        }
                    }

                    //Jump when it's possible
                    if (canJump)
                    {
                        bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                        if (isFrontBlocking && !isUpFrontBlocking)
                        {
                            if (jumpChecker.isGrounding)
                            {
                                Jump();
                            }
                        }
                    }

                    #endregion Platform Check
                }

                moveTime -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion IdleMove
    }

    /// <summary>
    /// 0 : 플레이어 보면 도망<br></br>
    /// 1 : 플레이어 신경 X<br></br>
    /// 2 : 플레이어와 거리 유지<br></br>
    /// 3 : 플레이어에게 돌진
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackMoveRoutine()
    {
        if (aggressionLevel == 0)
        {
            #region Run Away From Player

            while (true)
            {
                if (playerTransform == null)
                {
                    Debug.Log("Player is null");
                }
                int moveDirection = playerTransform.position.x - transform.position.x > 0 ? -1 : 1;


                #region Platform Check

                bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                if (canJump)
                {
                    if (isFrontBlocking)
                    {
                        bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                        if (!isUpFrontBlocking || isJumpAtDeadEnd)
                        {
                            if (jumpChecker.isGrounding)
                            {
                                Jump();
                            }
                        }
                    }
                }
                else
                {
                    if (isFrontBlocking)
                    {
                        if (isJumpAtDeadEnd && jumpChecker.isGrounding)
                        {
                            Jump();
                        }

                        rigid.velocity = new Vector2(0, rigid.velocity.y);

                        yield return new WaitForFixedUpdate();

                        continue;
                    }
                }

                #endregion Platform Check

                SetObjectDirection(moveDirection);

                rigid.velocity = new Vector2(moveDirection * moveSpeed * moveSpeedOnDetectMultiplier, rigid.velocity.y);

                yield return new WaitForFixedUpdate();
            }

            #endregion Run Away From Player
        }
        else if (aggressionLevel == 1)
        {
            #region Don't Care Player

            while (true)
            {
                float moveTime = 1f;

                int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

                SetObjectDirection(moveDirection);

                float stopProbability = Random.Range(0f, 1f);

                while (moveTime > 0f)
                {
                    if (stopProbability <= idleStopProbability)
                    {
                        rigid.velocity = new Vector2(0f, rigid.velocity.y);
                    }
                    else
                    {

                        rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                        #region Platform Check

                        bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                        bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                        //Stop in front of Cliff
                        if (canJump)
                        {
                            if (isFrontCliff)
                            {
                                bool isFrontHighCliff = CheckFrontCliff(moveDirection, jumpHeight + 0.3f, false);

                                if (isFrontCliff && isFrontHighCliff)
                                {
                                    if (isJumpAtDeadEnd)
                                    {
                                        if (jumpChecker.isGrounding)
                                        {
                                            Jump();
                                        }
                                    }

                                    rigid.velocity = new Vector2(0, rigid.velocity.y);

                                    yield return new WaitForFixedUpdate();

                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (isFrontCliff || isFrontBlocking)
                            {
                                rigid.velocity = new Vector2(0, rigid.velocity.y);

                                yield return new WaitForFixedUpdate();

                                break;
                            }
                        }

                        //Jump when it's possible
                        if (canJump)
                        {
                            bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                            if ((isFrontBlocking && !isUpFrontBlocking) || (isFrontBlocking && isJumpAtDeadEnd))
                            {
                                if (jumpChecker.isGrounding)
                                {
                                    Jump();
                                }
                            }
                        }

                        #endregion Platform Check
                    }

                    moveTime -= Time.deltaTime;

                    yield return new WaitForFixedUpdate();
                }
            }

            #endregion Don't Care Player
        }
        else if (aggressionLevel == 2)
        {
            #region Keep Distance From Player

            while (true)
            {
                float distanceToPlayer = Mathf.Abs(Vector2.Distance(playerTransform.position, transform.position));

                if (Mathf.Abs(distanceToPlayer - maintainDistance) < 0.5f)          // Stay Still
                {
                    if (!jumpChecker.isGrounding)
                    {
                        yield return new WaitForFixedUpdate();

                        continue;
                    }

                    int viewDirection = playerTransform.position.x - transform.position.x > 0 ? 1 : -1;

                    SetObjectDirection(viewDirection);

                    if (canJump && isJumpAtDeadEnd)
                    {
                        if (jumpChecker.isGrounding)
                        {
                            Jump();
                        }
                    }

                    rigid.velocity = new Vector2(0, rigid.velocity.y);

                    yield return new WaitForFixedUpdate();

                    continue;
                }

                if (distanceToPlayer > maintainDistance)                // Chase Player
                {
                    #region Chase Player

                    int moveDirection = playerTransform.position.x - transform.position.x > 0 ? 1 : -1;

                    if (Mathf.Abs(playerTransform.position.x - transform.position.x) < 0.5f)
                    {
                        moveDirection = 0;

                        if (canJump && isJumpAtDeadEnd)
                        {
                            if (jumpChecker.isGrounding)
                            {
                                Jump();
                            }
                        }
                    }

                    #region Platform Check

                    bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                    bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                    if (canJump)
                    {
                        if (isFrontCliff)
                        {
                            bool isFrontHighCliff = CheckFrontCliff(moveDirection, jumpHeight + 0.3f, false);

                            if (isFrontHighCliff)
                            {
                                if (isJumpAtDeadEnd)
                                {
                                    if (jumpChecker.isGrounding)
                                    {
                                        Jump();
                                    }
                                }

                                rigid.velocity = new Vector2(0, rigid.velocity.y);

                                yield return new WaitForFixedUpdate();

                                continue;
                            }
                        }

                        if (isFrontBlocking)
                        {
                            bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                            if (!isUpFrontBlocking || isJumpAtDeadEnd)
                            {
                                if (jumpChecker.isGrounding)
                                {
                                    Jump();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isFrontCliff || isFrontBlocking)
                        {
                            rigid.velocity = new Vector2(0, rigid.velocity.y);

                            yield return new WaitForFixedUpdate();

                            continue;
                        }
                    }

                    #endregion Platform Check

                    SetObjectDirection(moveDirection);

                    rigid.velocity = new Vector2(moveDirection * moveSpeed * moveSpeedOnDetectMultiplier, rigid.velocity.y);

                    #endregion Chase Player
                }
                else if (distanceToPlayer < maintainDistance)
                {
                    #region Run Away From Player

                    int moveDirection = playerTransform.position.x - transform.position.x > 0 ? -1 : 1;

                    #region Platform Check

                    bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                    bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                    if (canJump)
                    {
                        if (isFrontCliff)
                        {
                            bool isFrontHighCliff = CheckFrontCliff(moveDirection, jumpHeight + 0.3f, false);

                            if (isFrontHighCliff)
                            {
                                if (isJumpAtDeadEnd)
                                {
                                    if (jumpChecker.isGrounding)
                                    {
                                        Jump();
                                    }
                                }

                                rigid.velocity = new Vector2(0, rigid.velocity.y);

                                yield return new WaitForFixedUpdate();

                                continue;
                            }
                        }

                        if (isFrontBlocking)
                        {
                            bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                            if (!isUpFrontBlocking || isJumpAtDeadEnd)
                            {
                                if (jumpChecker.isGrounding)
                                {
                                    Jump();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isFrontCliff || isFrontBlocking)
                        {
                            rigid.velocity = new Vector2(0, rigid.velocity.y);

                            yield return new WaitForFixedUpdate();

                            continue;
                        }
                    }

                    #endregion Platform Check

                    SetObjectDirection(moveDirection);

                    rigid.velocity = new Vector2(moveDirection * moveSpeed * moveSpeedOnDetectMultiplier, rigid.velocity.y);

                    #endregion Run Away From Player
                }

                yield return new WaitForFixedUpdate();
            }
            
            #endregion Keep Distance From Player
        }
        else if (aggressionLevel == 3)
        {
            #region Run to Player

            while (true)
            {
                int moveDirection = playerTransform.position.x - transform.position.x > 0 ? 1 : -1;

                SetObjectDirection(moveDirection);

                if (Mathf.Abs(playerTransform.position.x - transform.position.x) < 2f)
                {
                    moveDirection = 0;

                    if (canJump && isJumpAtDeadEnd)
                    {
                        if (jumpChecker.isGrounding)
                        {
                            Jump();
                        }
                    }
                }

                #region Platform Check

                if (canJump)
                {
                    bool isFrontBlocking = CheckFrontBlocking(moveDirection, frontRayVerticalOffset, frontRayLength);

                    if (isFrontBlocking)
                    {
                        bool isUpFrontBlocking = CheckFrontBlocking(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                        if (!isUpFrontBlocking || isJumpAtDeadEnd)
                        {
                            if (jumpChecker.isGrounding)
                            {
                                Jump();
                            }
                        }
                    }
                }

                #endregion Platform Check

                rigid.velocity = new Vector2(moveDirection * moveSpeed * moveSpeedOnDetectMultiplier, rigid.velocity.y);

                yield return new WaitForFixedUpdate();
            }

            #endregion Run to Player
        }
        else
        {
            Debug.Log("Error : Enemy " + this.name + "'s aggresion level is not correct");
        }
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
    }

    public void PauseMove(float time)
    {
        StartCoroutine(PauseMoveRoutine(time));
    }

    private IEnumerator PauseMoveRoutine(float time)
    {
        float firstMoveSpeed = moveSpeed;

        moveSpeed = 0f;

        yield return new WaitForSeconds(time);

        moveSpeed = firstMoveSpeed;
    }

    private void ChangeState(State state) => this.state = state;

    private void SetObjectDirection(int moveDirection)
    {
        if (moveDirection < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
    }

    private void OnDrawGizmos()
    {
        if (myCollider == null)
        {
            myCollider = GetComponent<Collider2D>();
        }

        int moveDirection;
        if (transform.localScale.x > 0)
        {
            moveDirection = -1;
        }
        else
        {
            moveDirection = 1;
        }

        //Down Ray
        Debug.DrawRay(new Vector2(transform.position.x + moveDirection * downRayHorizontalOffset, transform.position.y), Vector3.down * downRayLength, new Color(1f, 0f, 1f));

        //Front Ray
        Debug.DrawRay(transform.position + new Vector3(0f, frontRayVerticalOffset, 0f), new Vector3(moveDirection * frontRayLength, 0, 0), new Color(1f, 0f, 0f));

        if (canJump)
        {
            //Down Platform Ray
            Debug.DrawRay(new Vector2(transform.position.x + moveDirection * downRayHorizontalOffset, myCollider.bounds.min.y), Vector3.down * (jumpHeight + 0.3f), new Color(0f, 1f, 0f));

            //Jump Ray
            Debug.DrawRay(new Vector3(transform.position.x, myCollider.bounds.min.y + jumpHeight + 0.3f, 0f), new Vector3(moveDirection * jumpFrontRayLength, 0f, 0f), new Color(0f, 0f, 1));
        }
    }

    private bool CheckFrontCliff(int direction, float length, bool isUsingTransformPosition = true)
    {
        Vector2 originVector;

        if (isUsingTransformPosition)
        {
            //Use Y axis as Middle of Collider
            originVector = new Vector2(transform.position.x + direction * downRayHorizontalOffset, transform.position.y);
        }
        else
        {
            //Use Y axis as Bottom of Collider
            originVector = new Vector2(transform.position.x + direction * downRayHorizontalOffset, myCollider.bounds.min.y);
        }

        RaycastHit2D downRayHit = Physics2D.Raycast(originVector, Vector3.down, length, blockingLayerMask);

        if (downRayHit.collider == null)
            return true;
        else
            return false;
    }

    private bool CheckFrontBlocking(int direction, float offset, float length, bool isUsingTransformPosition = true)
    {
        Vector2 originVector;

        if (isUsingTransformPosition)
            originVector = transform.position + new Vector3(0f, offset, 0f);
        else
            originVector = new Vector3(transform.position.x, myCollider.bounds.min.y + offset, 0f);

        RaycastHit2D frontRayHit = Physics2D.Raycast(originVector, Vector3.right * direction, length, blockingLayerMask);

        if (frontRayHit.collider != null)
            return true;
        else
            return false;
    }
}