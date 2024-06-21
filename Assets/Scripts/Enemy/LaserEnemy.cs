using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserEnemy : Enemy
{
    private GameObject headObj;
    private SpriteRenderer headSpriteRenderer;

    private Vector2 targetVec;

    [Header("Body Part Settings")]
    [SerializeField]
    private GameObject bodyPartsObj;

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
        base.Init();
        StartCoroutine(IdleMove());

        headObj = bodyPartsObj.transform.Find("Head").gameObject;
        headSpriteRenderer = headObj.GetComponent<SpriteRenderer>();

        onDie += () => gameObject.SetActive(false);
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
            LookAtPlayer(headObj, headSpriteRenderer);
            SetWarningLaser();
        }
    }

    protected override GameObject Attack(Vector2 dir)
    {
        var obj = base.Attack(dir);
        Laser laser = obj.GetComponent<Laser>();
        laser.Activate(dir);
        obj.GetComponent<LineRenderer>().material.color = new Color(128, 0, 0);
        return obj;
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
                bodyPartsObj.transform.localScale = new Vector3(1, 1, 1);
                if (IsPlayerDetected)
                {
                    headSpriteRenderer.flipX = false;
                }
            }
            else if (moveDir > 0)
            {
                bodyPartsObj.transform.localScale = new Vector3(-1, 1, 1);
                if (IsPlayerDetected)
                {
                    headSpriteRenderer.flipX = true;
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
