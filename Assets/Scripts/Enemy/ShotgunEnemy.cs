using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunEnemy : Enemy
{
    private SpriteRenderer headSpriteRenderer;

    [Header("Body Part Settings")]
    [SerializeField]
    private GameObject headObj;
    [SerializeField]
    private GameObject bodyObj;

    [Header("Move Ray Settings")]
    [SerializeField]
    private float platformCheckRayGap;
    [SerializeField]
    private float platformCheckRayDistance;
    [SerializeField]
    private float lampCheckRayDistance;

    [Header("Attack Settings")]
    [SerializeField]
    private float aimAccuracy;

    private void Start()
    {
        base.Init();
        StartCoroutine(IdleMove());

        headSpriteRenderer = headObj.GetComponent<SpriteRenderer>();

        onDie += () => gameObject.SetActive(false);
    }

    private void Update()
    {
        if (IsAttackPossible()) Attack(GetTargetDir(Player));
        if (IsPlayerDetected) LookAtPlayer(headObj, headSpriteRenderer);
    }

    protected override void Attack(Vector2 dir)
    {
        float aimAngle = (90 - (aimAccuracy * 9 * 0.1f)) * Mathf.Deg2Rad;
        Vector3 dir2 = new Vector3(
                dir.x * Mathf.Cos(aimAngle) - dir.y * Mathf.Sin(aimAngle),
                dir.x * Mathf.Sin(aimAngle) + dir.y * Mathf.Cos(aimAngle));
        Vector3 dir3 = new Vector3(
                dir.x * Mathf.Cos(-aimAngle) - dir.y * Mathf.Sin(-aimAngle),
                dir.x * Mathf.Sin(-aimAngle) + dir.y * Mathf.Cos(-aimAngle));

        base.Attack(dir);
        base.Attack(dir2);
        base.Attack(dir3);
    }

    protected override IEnumerator IdleMove()
    {
        while (true)
        {
            float moveTime = Random.Range(1f, 3f);
            int moveDir = Random.Range(-1, 2);

            if (moveDir < 0)
            {
                bodyObj.transform.localScale = new Vector3(1, 1, 1);
                if (IsPlayerDetected)
                {
                    headSpriteRenderer.flipX = false;
                }
            }
            else if (moveDir > 0)
            {
                bodyObj.transform.localScale = new Vector3(-1, 1, 1);
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
}
