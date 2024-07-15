using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private int aggressionLevel = 1;
    [SerializeField] private LayerMask groundingLayerMask;

    [Header("Move Pattern Settings")]
    [SerializeField] private bool canJump = false;
    [SerializeField] private bool canFly = false;
    [SerializeField] private bool detectOnHit = true;
    [SerializeField] private bool isStationaryOnShooting = true;
    [SerializeField] private float moveSpeedOnDetectMultiplier = 1f;

    [Header("Attack Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 100f;

    [Header("Move Ray Settings")]
    [SerializeField] private float downRayHorizontalOffset = 1f;
    [SerializeField] private float downRayLength = 1.5f;
    [SerializeField] private float frontRayVerticalOffset = -0.5f;
    [SerializeField] private float frontRayLength = 1.5f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpFrontRayLength = 2f;

    private Health health;
    private Rigidbody2D rigid;
    private Enemy enemy;
    private Collider2D myCollider;
    private JumpChecker jumpChecker;
    private PlayerDetector playerDetector;

    private bool isJumping;
    private bool isPlayerDetected;
    public bool IsPlayerDetected { get => isPlayerDetected; }

    private Transform playerTransform;
    private State state;
    private Coroutine currentMoveRoutine;

    private void Awake()
    {
        health = GetComponent<Health>();
        rigid = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        myCollider = GetComponent<Collider2D>();

        playerDetector = GetComponentInChildren<PlayerDetector>();
        jumpChecker = GetComponentInChildren<JumpChecker>();
    }

    private void Start()
    {
        health.onDie += () => gameObject.SetActive(false);
        isJumping = false;
        isPlayerDetected = false;

        currentMoveRoutine = StartCoroutine(IdleMoveRoutine());
    }

    private void Update()
    {
        ExcuteStateAction();
        CheckAttackRange();
    }

    public void OnPlayerDetected(Transform playerTransform)
    {
        isPlayerDetected = true;
        this.playerTransform = playerTransform;

        ChangeState(State.Attack);

        StopCoroutine(currentMoveRoutine);
        currentMoveRoutine = StartCoroutine(AttackMoveRoutine());
    }

    public void OnPlayerLost()
    {
        isPlayerDetected = false;
        this.playerTransform = null;

        ChangeState(State.Idle);

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
                    enemy.Attack(playerTransform);
                }
                break;
            case State.Chase:
            case State.Dead:
                break;
        }
    }

    private void CheckAttackRange()
    {
        if (state == State.Attack && playerTransform != null)
        {
            if (Vector3.Distance(playerTransform.position, transform.position) > detectionRange)
            {
                OnPlayerLost();
            }
        }
    }

    private IEnumerator IdleMoveRoutine()
    {
        while (true)
        {
            float moveTime = 1f;
            int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

            SetObjectDirection(moveDirection);

            while (moveTime > 0f)
            {
                rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                #region Platform Check

                bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                bool isFrontPlatform = CheckFrontPlatform(moveDirection, frontRayVerticalOffset, frontRayLength);

                //Stop in front of Cliff
                if (canJump)
                {
                    if (jumpChecker.isGrounding)
                    {
                        bool isFrontHighCliff = CheckFrontCliff(moveDirection, jumpHeight + 0.3f, false);

                        if (isFrontCliff && isFrontHighCliff)
                        {
                            rigid.velocity = new Vector2(0, rigid.velocity.y);
                            yield return new WaitForSeconds(1f);
                            break;
                        }
                    }
                }
                else
                {
                    if (isFrontCliff)
                    {
                        rigid.velocity = new Vector2(0, rigid.velocity.y);
                        yield return new WaitForSeconds(1f);
                        break;
                    }
                }
                
                //Jump when it's possible
                if (canJump)
                {
                    bool isUpFrontPlatform = CheckFrontPlatform(moveDirection, jumpHeight + 0.3f, jumpFrontRayLength, false);

                    if (isFrontPlatform && !isUpFrontPlatform)
                    {
                        if (jumpChecker.isGrounding)
                        {
                            Jump();
                        }
                    }
                }

                #endregion Platform Check

                moveTime -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
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
                if (canJump && jumpChecker.isGrounding)
                {
                    Jump();
                }

                int moveDirection = playerTransform.position.x - transform.position.x > 0 ? -1 : 1;

                #region Platform Check
                bool isFrontPlatform = CheckFrontPlatform(moveDirection, frontRayVerticalOffset, frontRayLength);

                if (isFrontPlatform)
                {
                    rigid.velocity = new Vector2(0, rigid.velocity.y);

                    yield return new WaitForSeconds(Time.deltaTime);

                    continue;
                }
                #endregion Platform Check

                SetObjectDirection(moveDirection);

                rigid.velocity = new Vector2(moveDirection * moveSpeed * moveSpeedOnDetectMultiplier, rigid.velocity.y);

                yield return new WaitForSeconds(Time.deltaTime);
            }
            #endregion
        }
        else if (aggressionLevel == 1)
        {
            #region Don't Care Player

            while (true)
            {
                float moveTime = 1f;
                int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

                SetObjectDirection(moveDirection);

                while (moveTime > 0f)
                {
                    if (canJump && jumpChecker.isGrounding)
                    {
                        Jump();
                    }

                    rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);


                    #region Platform Check

                    bool isFrontCliff = CheckFrontCliff(moveDirection, downRayLength);

                    bool isFrontPlatform = CheckFrontPlatform(moveDirection, frontRayVerticalOffset, frontRayLength);

                    if (isFrontCliff || isFrontPlatform)
                    {
                        rigid.velocity = new Vector2(0, rigid.velocity.y);
                        yield return new WaitForSeconds(1f);
                        break;
                    }
                    #endregion Platform Check

                    moveTime -= Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
            #endregion
        }
        else if (aggressionLevel == 2)
        {

        }
        else if (aggressionLevel == 3)
        {

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
        Debug.DrawRay(new Vector2(transform.position.x + moveDirection * downRayHorizontalOffset, transform.position.y), Vector3.down * downRayLength, new Color(1, 0, 1));

        //Front Ray
        Debug.DrawRay(transform.position + new Vector3(0f, frontRayVerticalOffset, 0f), new Vector3(moveDirection * frontRayLength, 0, 0), new Color(1, 0, 0));

        if (canJump)
        {
            //Down Platform Ray
            Debug.DrawRay(new Vector2(transform.position.x + 0.3f + moveDirection * downRayHorizontalOffset, myCollider.bounds.min.y), Vector3.down * (jumpHeight + 0.3f), new Color(0.5f, 0, 0.5f));

            //Jump Ray
            Debug.DrawRay(new Vector3(transform.position.x, myCollider.bounds.min.y + jumpHeight + 0.3f, 0f), new Vector3(moveDirection * jumpFrontRayLength, 0, 0), new Color(0, 0, 1));
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

        RaycastHit2D downRayHit = Physics2D.Raycast(originVector, Vector3.down, length, groundingLayerMask);

        if (downRayHit.collider == null)
            return true;
        else
            return false;
    }

    private bool CheckFrontPlatform(int direction, float offset, float length, bool isUsingTransformPosition = true)
    {
        Vector2 originVector;

        if (isUsingTransformPosition)
            originVector = transform.position + new Vector3(0f, offset, 0f);
        else
            originVector = new Vector3(transform.position.x, myCollider.bounds.min.y + offset, 0f);

        RaycastHit2D frontRayHit = Physics2D.Raycast(originVector, Vector3.right * direction, length, groundingLayerMask);

        if (frontRayHit.collider != null)
            return true;
        else
            return false;
    }
}