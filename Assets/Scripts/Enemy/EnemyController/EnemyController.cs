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
    [SerializeField] private bool detectOnHit = true;
    [SerializeField] private bool isStationaryOnShooting = true;
    [SerializeField] private bool canJump = false;
    [SerializeField] private bool canFly = false;

    [Header("Attack Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 100f;

    [Header("Move Ray Settings")]
    [SerializeField] private float downCheckRayHorizontalOffset = 1f;
    [SerializeField] private float downCheckRayLength = 1.5f;
    [SerializeField] private float frontCheckRayVerticalOffset = -0.5f;
    [SerializeField] private float frontCheckRayLength = 1.5f;

    private Health health;
    private Rigidbody2D rigid;
    private Enemy enemy;
    private PlayerDetector playerDetector;

    private bool isCool;
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
        playerDetector = GetComponentInChildren<PlayerDetector>();
    }

    private void Start()
    {
        health.onDie += () => gameObject.SetActive(false);
        isCool = false;
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
                Vector2 frontVec = new Vector2(transform.position.x + moveDirection * downCheckRayHorizontalOffset, transform.position.y);

                RaycastHit2D downRayHit = Physics2D.Raycast(frontVec, Vector3.down, downCheckRayLength, groundingLayerMask);

                RaycastHit2D frontRayHit = Physics2D.Raycast(transform.position + new Vector3(0f, frontCheckRayVerticalOffset, 0f), Vector3.right * moveDirection, frontCheckRayLength, groundingLayerMask);

                if (downRayHit.collider == null || frontRayHit.collider != null)
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
                int moveDirection = playerTransform.position.x - transform.position.x > 0 ? -1 : 1;

                SetObjectDirection(moveDirection);

                rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                #region Platform Check
                RaycastHit2D frontRayHit = Physics2D.Raycast(transform.position + new Vector3(0f, frontCheckRayVerticalOffset, 0f), Vector3.right * moveDirection, frontCheckRayLength, groundingLayerMask);

                if (frontRayHit.collider != null)
                {
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                    break;
                }

                yield return new WaitForSeconds(Time.deltaTime);
                #endregion Platform Check
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
                    rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                    #region Platform Check
                    Vector2 frontVector = new Vector2(transform.position.x + moveDirection * downCheckRayHorizontalOffset, transform.position.y);

                    RaycastHit2D downRayHit = Physics2D.Raycast(frontVector, Vector3.down, downCheckRayLength, groundingLayerMask);

                    RaycastHit2D frontRayHit = Physics2D.Raycast(transform.position + new Vector3(0f, frontCheckRayVerticalOffset, 0f), Vector3.right * moveDirection, frontCheckRayLength, groundingLayerMask);

                    if (downRayHit.collider == null || frontRayHit.collider != null)
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

    private bool ScanCliffAhead()
    {
        return false;
    }

    private bool ScanPlatformAhead()
    {
        return false;
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
        int moveDirection;
        if (transform.localScale.x > 0)
        {
            moveDirection = -1;
        }
        else
        {
            moveDirection = 1;
        }
        Vector2 frontVec = new Vector2(transform.position.x + moveDirection * downCheckRayHorizontalOffset, transform.position.y);
        Debug.DrawRay(frontVec, Vector3.down * downCheckRayLength, new Color(1, 0, 1));
        Debug.DrawRay(transform.position + new Vector3(0f, frontCheckRayVerticalOffset, 0f), new Vector3(moveDirection * frontCheckRayLength, 0, 0), new Color(1, 0, 0));

    }
}