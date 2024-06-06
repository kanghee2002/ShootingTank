using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    [Header("Body Part Settings")]
    [SerializeField]
    private GameObject headObj;
    [SerializeField]
    private SpriteRenderer headSpriteRenderer;
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
    private float explosionRadius;
    [SerializeField]
    private float explosionDelay;
    [SerializeField]
    private Explosion explosion;

    private IEnumerator curMoveCoroutine;

    private void Start()
    {
        curMoveCoroutine = IdleMove();
        StartCoroutine(curMoveCoroutine);
    }

    private void Update()
    {
        if (IsPlayerDetected)
        {
            LookAtPlayer(headObj, headSpriteRenderer);
        }
    }

    public override void OnPlayerDetected(Transform player)
    {
        base.OnPlayerDetected(player);
        StopCoroutine(curMoveCoroutine);
        curMoveCoroutine = ChasePlayer();
        StartCoroutine(curMoveCoroutine);
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
            }
            else if (moveDir > 0)
            {
                bodyObj.transform.localScale = new Vector3(-1, 1, 1);
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

    private IEnumerator ChasePlayer()
    {
        while (true)
        {
            int moveDir;
            float distance = Player.position.x - transform.position.x;

            if (Mathf.Abs(distance) < 0.3f) moveDir = 0;
            else if (distance < 0) moveDir = -1;
            else moveDir = 1;

            if (moveDir < 0)
            {
                bodyObj.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveDir > 0)
            {
                bodyObj.transform.localScale = new Vector3(-1, 1, 1);
            }

            rigid.velocity = new Vector2(moveDir * moveSpeed, rigid.velocity.y);

            if ((Player.position - transform.position).magnitude <= explosionRadius)
            {
                StartCoroutine(Explode());
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Explode()
    {
        StopCoroutine(curMoveCoroutine);

        yield return new WaitForSeconds(explosionDelay);

        explosion.AddTargetLayerMask("Player");
        explosion.DamageAmount = damageValue;
        explosion.gameObject.SetActive(true);
        explosion.transform.SetParent(null);
        gameObject.SetActive(false);
    }
}