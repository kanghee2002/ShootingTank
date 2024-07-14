using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Enemy))]
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
    [SerializeField] private int aggressionLevel = 1;
    [SerializeField] private LayerMask groundingLayerMask;

    [Header("Move Pattern Settings")]
    [SerializeField] private bool detectOnHit = true;
    [SerializeField] private bool isStationaryOnShooting = true;

    [Header("Attack Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 100f;

    [Header("Move Ray Settings")]
    [SerializeField] private float platformCheckRayGap = 1f;
    [SerializeField] private float platformCheckRayDistance = 1.5f;
    [SerializeField] private float lampCheckRayDistance = 1.5f;

    private Health health;
    private Rigidbody2D rigid;
    private Enemy enemy;

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
        int lastMoveDirection = -1;

        while (true)
        {
            float moveTime = 1f;
            int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

            Vector3 oppositeScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);

            if (lastMoveDirection * moveDirection < 0)      // 마지막으로 향한 방향과 현재 가고자 하는 방향이 다를 경우 (-) 
                transform.localScale = oppositeScale;

            lastMoveDirection = moveDirection;

            while (moveTime > 0f)
            {
                rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                #region Platform Check
                Vector2 frontVec = new Vector2(rigid.position.x + moveDirection * platformCheckRayGap, rigid.position.y);

                Debug.DrawRay(frontVec, Vector3.down * platformCheckRayDistance, new Color(1, 0, 1));
                RaycastHit2D rayHitPlatform = Physics2D.Raycast(frontVec, Vector3.down, platformCheckRayDistance, groundingLayerMask);

                Debug.DrawRay(transform.position, new Vector3(moveDirection * lampCheckRayDistance, 0, 0), new Color(1, 0, 0));
                RaycastHit2D rayHitLamp = Physics2D.Raycast(transform.position, Vector3.right * moveDirection, lampCheckRayDistance, groundingLayerMask);

                if (rayHitPlatform.collider == null || rayHitLamp.collider != null)
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

        }
        else if (aggressionLevel == 1)
        {
            #region Don't Care Player
            int lastMoveDirection = -1;

            while (true)
            {
                float moveTime = 1f;
                int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

                Vector3 oppositeScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);

                if (lastMoveDirection * moveDirection < 0)      // 마지막으로 향한 방향과 현재 가고자 하는 방향이 다를 경우 (-) 
                    transform.localScale = oppositeScale;

                lastMoveDirection = moveDirection;

                while (moveTime > 0f)
                {
                    rigid.velocity = new Vector2(moveDirection * moveSpeed, rigid.velocity.y);

                    #region Platform Check
                    Vector2 frontVec = new Vector2(rigid.position.x + moveDirection * platformCheckRayGap, rigid.position.y);

                    Debug.DrawRay(frontVec, Vector3.down * platformCheckRayDistance, new Color(1, 0, 1));
                    RaycastHit2D rayHitPlatform = Physics2D.Raycast(frontVec, Vector3.down, platformCheckRayDistance, groundingLayerMask);

                    Debug.DrawRay(transform.position, new Vector3(moveDirection * lampCheckRayDistance, 0, 0), new Color(1, 0, 0));
                    RaycastHit2D rayHitLamp = Physics2D.Raycast(transform.position, Vector3.right * moveDirection, lampCheckRayDistance, groundingLayerMask);

                    if (rayHitPlatform.collider == null || rayHitLamp.collider != null)
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

    }

    private bool ScanPlatformAhead()
    {

    }

    private void ChangeState(State state) => this.state = state;
}