using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Bullet
{
    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;

    private float firstWidth;

    [SerializeField] private List<string> blockLayerMaskList;
    [SerializeField] private List<string> nonBlockingTagList;

    [SerializeField] private float distance;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        firstWidth = lineRenderer.startWidth;
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    public void Activate(Vector3 dir)
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));

        Vector3 startPosition = transform.position, endPosition;

        RaycastHit2D rayHit = Physics2D.Raycast(startPosition, dir, 
            (startPosition + dir * distance).magnitude, LayerMask.GetMask(blockLayerMaskList.ToArray()));

        bool isNonBlocking = false;

        if (rayHit)
        {
            foreach (string tag in nonBlockingTagList)
            {
                if (rayHit.collider.CompareTag(tag))
                {
                    isNonBlocking = true;
                    break;
                }
            }
        }

        if (rayHit && !isNonBlocking)
        {
            endPosition = rayHit.point;
        }
        else
        {
            endPosition = startPosition + dir * distance;
        }

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        lineRenderer.enabled = true;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float width = firstWidth, elapsedTime = 0f, decreaseSpeed = width / lifeTIme;

        StopCoroutine(checkLifeTimeRoutine);
        
        while (elapsedTime < lifeTIme)
        {
            elapsedTime += Time.deltaTime;
            width -= decreaseSpeed * Time.deltaTime;

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            SetPolygonCollider(width);

            yield return new WaitForFixedUpdate();
        }

        lineRenderer.startWidth = firstWidth;
        lineRenderer.endWidth = firstWidth;
        lineRenderer.enabled = false;

        DestroyBullet();
    }

    private void SetPolygonCollider(float width)
    {
        Vector2 startPos = lineRenderer.GetPosition(0) - transform.position;
        Vector2 endPos = lineRenderer.GetPosition(1) - transform.position;

        Vector2 directionVector = (endPos - startPos).normalized;
        Vector2 perendicularVector = Quaternion.AngleAxis(90, Vector3.forward) * directionVector;

        List<Vector2> points = new()
        {
            startPos + perendicularVector * (width / 2f),
            startPos - perendicularVector * (width / 2f),
            endPos - perendicularVector * (width / 2f),
            endPos + perendicularVector * (width / 2f),
        };

        polygonCollider.points = points.ToArray();
    }

    public void SetLaserWidth(float width)
    {
        firstWidth = width;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public void SetDuration(float duration)
    {
        lifeTIme = duration;
    }

    public bool AddBlockLayerMask(string layerMask)
    {
        if (blockLayerMaskList.Contains(layerMask))
        {
            return false;
        }
        else
        {
            blockLayerMaskList.Add(layerMask);
            return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (collision.CompareTag(targetTag))
            {
                if (collision.TryGetComponent(out CoreHealth coreHealth))
                {
                    bool hasKilled = coreHealth.TakeDamage(FinalDamageOnCoreHit);

                    if (firedWeapon)
                    {
                        firedWeapon.onCoreHit?.Invoke(FinalDamageOnCoreHit * coreHealth.CoreDamageMultiplier);

                        if (hasKilled)
                        {
                            firedWeapon.onKill?.Invoke();
                        }
                    }
                }
                else if (collision.TryGetComponent(out Health health))
                {
                    bool hasKilled = health.TakeDamage(FinalDamage);

                    if (firedWeapon)
                    {
                        firedWeapon.onHit?.Invoke(FinalDamage);

                        if (hasKilled)
                        {
                            firedWeapon.onKill?.Invoke();
                        }
                    }
                }
            }
        }
    }
}
