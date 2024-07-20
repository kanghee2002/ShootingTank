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

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        StartCoroutine(Explode());
        isCool = true;
        return true;
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

        explosion.transform.SetParent(null);
        explosion.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}