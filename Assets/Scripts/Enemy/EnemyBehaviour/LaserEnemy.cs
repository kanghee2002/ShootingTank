using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserEnemy : Enemy
{
    private Vector2 targetVec;

    [Header("Attack Settings")]
    [SerializeField]
    private float weaponLength;

    [Header("Move Ray Settings")]
    [SerializeField]
    private float platformCheckRayGap;
    [SerializeField]
    private float platformCheckRayDistance;
    [SerializeField]
    private float lampCheckRayDistance;

    [Header("Warning Laser")]
    [SerializeField]
    private WarningLaser warningLaser;

    private void Start()
    {
        StartCoroutine(IdleMove());

        health.onDie += () => gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsAttackPossible())
        {
            StartCoroutine(DelayAttack());
            isCool = true;
        }
        if (IsPlayerDetected)
        {
            //LookAtPlayer(headObj, headSpriteRenderer);
            SetWarningLaser();
        }
    }

    protected override void Attack(Vector3 direction)
    {
        var obj = objectPool.GetBullet();

        obj.transform.position = transform.position + direction * weaponLength;

        Laser laser = obj.GetComponent<Laser>();
        laser.FinalDamage = damageValue;
        laser.AddTargetTag("Player");
        laser.Activate(direction);

        obj.GetComponent<LineRenderer>().material.color = new Color(128, 0, 0);

        StartCoroutine(CheckCoolTime(coolTime));
        isCool = true;
    }

    private IEnumerator DelayAttack()
    {
        float delay = warningLaser.StartLuminesce();

        yield return new WaitForSeconds(delay);

        if ((transform.position - Player.position).magnitude <= warningLaser.MaxDistance)
        {
            Attack(GetTargetDir(Player));
        }
        else
        {
            isCool = false;
        }
    }

    protected override IEnumerator IdleMove()
    {
        while (true)
        {
            float moveTime = Random.Range(1f, 3f);
            int moveDir = Random.Range(-1, 2);

            if (moveDir < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
                if (IsPlayerDetected)
                {
                    //headSpriteRenderer.flipX = false;
                }
            }
            else if (moveDir > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
                if (IsPlayerDetected)
                {
                    //headSpriteRenderer.flipX = true;
                }
            }

            while (moveTime > 0f)
            {
                rigid.velocity = new Vector2(moveDir * moveSpeed, rigid.velocity.y);

                //Platform Check
                Vector2 frontVec = new Vector2(rigid.position.x + moveDir * platformCheckRayGap, rigid.position.y);

                Debug.DrawRay(frontVec, Vector3.down * platformCheckRayDistance, new Color(1, 0, 1));
                RaycastHit2D rayHitPlatform = Physics2D.Raycast(frontVec, Vector3.down, platformCheckRayDistance, LayerMask.GetMask("Platform"));

                Debug.DrawRay(transform.position, new Vector3(moveDir * lampCheckRayDistance, 0, 0), new Color(1, 0, 0));
                RaycastHit2D rayHitLamp = Physics2D.Raycast(transform.position, Vector3.right * moveDir, lampCheckRayDistance, LayerMask.GetMask("Platform"));

                if (rayHitPlatform.collider == null || rayHitLamp.collider != null)
                {
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                    yield return new WaitForSeconds(1f);
                    break;
                }

                moveTime -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }

    private void SetWarningLaser()
    {
        warningLaser.SetPosition(transform.position, Player.position);
    }
}
