using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ExplosiveEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private float explosionDelay;
    [SerializeField]
    private Explosion explosion;

    private SpriteRenderer spriteRenderer;

    private float fadeInOutSpeed = 3f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    /*private IEnumerator ChasePlayer()
    {
        while (true)
        {
            int moveDir;
            float distance = playerTransform.position.x - transform.position.x;

            if (Mathf.Abs(distance) < 0.3f) moveDir = 0;
            else if (distance < 0) moveDir = -1;
            else moveDir = 1;

            if (moveDir < 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
            }
            else if (moveDir > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
            }

            rigid.velocity = new Vector2(moveDir * moveSpeed, rigid.velocity.y);

            if ((playerTransform.position - transform.position).magnitude <= explosionRadius)
            {
                Attack(Vector3.zero);
            }

            yield return new WaitForFixedUpdate();
        }
    }*/

    public override void Attack(Transform playerTransform)
    {
        StartCoroutine(Explode());
    }

    private IEnumerator RepeatFadeInOut()
    {
        IEnumerator curCoroutine = null;
        while (gameObject.activeSelf == true)
        {
            if (spriteRenderer.color.r > 0.99f)
            {
                if (curCoroutine != null)
                {
                    StopCoroutine(curCoroutine);
                }
                curCoroutine = FadeOut();
                StartCoroutine(curCoroutine);
            }
            else if (spriteRenderer.color.r < 0.01f)
            {

                if (curCoroutine != null)
                {
                    StopCoroutine(curCoroutine);
                }
                curCoroutine = FadeIn();
                StartCoroutine(curCoroutine);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FadeOut()
    {
        float curColorNum = spriteRenderer.color.r;
        while (curColorNum > 0f)
        {
            curColorNum -= fadeInOutSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(curColorNum, curColorNum, curColorNum);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FadeIn()
    {
        float curColorNum = spriteRenderer.color.r;
        while (curColorNum < 1f)
        {
            curColorNum += fadeInOutSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(curColorNum, curColorNum, curColorNum);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Explode()
    {
        StartCoroutine(RepeatFadeInOut());

        yield return new WaitForSeconds(explosionDelay);

        explosion.AddTargetLayerMask(Settings.playerTag);
        explosion.AddTargetLayerMask(Settings.enemyTag);
        explosion.DamageAmount = damageValue;
        explosion.gameObject.SetActive(true);
        explosion.transform.SetParent(null);
        gameObject.SetActive(false);
    }
}