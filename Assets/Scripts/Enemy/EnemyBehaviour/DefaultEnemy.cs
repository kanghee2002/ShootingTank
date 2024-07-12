using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DefaultEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float weaponLength;
    [SerializeField] private float attackRange;

    [Header("Move Ray Settings")]
    [SerializeField] private float platformCheckRayGap;
    [SerializeField] private float platformCheckRayDistance;
    [SerializeField] private float lampCheckRayDistance;

    private Coroutine currentStateRoutine;

    private void Start()
    {
        state = State.Idle;

        health.onDie += () => gameObject.SetActive(false);

        currentStateRoutine = StartCoroutine(IdleRoutine());
    }

    private void Update()
    {
        ExcuteStateAction();
    }

    public override void OnPlayerDetected(Transform playerTransform)
    {
        base.OnPlayerDetected(playerTransform);
        ChangeState(State.Attack);
    }

    public override void OnPlayerLost()
    {
        base.OnPlayerLost();
        ChangeState(State.Idle);
    }

    private void ExcuteStateAction()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Attack:
                if (IsAttackPossible())
                {
                    Attack(GetTargetDirection(playerTransform));
                }
                break;
            case State.Chase:
            case State.Dead:
                break;
        }
    }

    protected override void Attack(Vector3 direction)
    {
        var obj = objectPool.GetBullet();
        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        objectPool.LookAtDirection(obj, direction);

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.FinalDamage = damageValue;
        bullet.AddTargetTag("Player");
        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;
    }

    protected IEnumerator IdleRoutine()
    {
        int lastMoveDirection = -1;

        while (true)
        {
            float moveTime = 1f;
            int moveDirection = Random.Range(0, 1f) > 0.5f ? 1 : -1;

            Vector3 oppositeScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);

            if (playerTransform == null && lastMoveDirection * moveDirection < 0)      // 마지막으로 향한 방향과 현재 가고자 하는 방향이 다를 경우 (-) 
                transform.localScale = oppositeScale;

            lastMoveDirection = moveDirection;

            while (moveTime > 0f)
            {
                if (IsPlayerDetected)
                {
                    float localScaleX = Mathf.Abs(transform.localScale.x);

                    if (playerTransform.position.x < transform.position.x)
                        transform.localScale = new Vector3(localScaleX, transform.localScale.y, 1);
                    else
                        transform.localScale = new Vector3(-localScaleX, transform.localScale.y, 1);
                }

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
}
