using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Bullet
{
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;

    private float firstWidth;

    [SerializeField]
    private float distance;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        firstWidth = lineRenderer.startWidth;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Activate(Vector3 dir)
    {
        StartCoroutine(FadeOut());

        Vector3 startPosition = transform.position, endPosition;

        RaycastHit2D rayHit = Physics2D.Raycast(startPosition, dir, 
            (startPosition + dir * distance).magnitude, LayerMask.GetMask("Platform"));

        if (rayHit)
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
    }

    private IEnumerator FadeOut()
    {
        float width = firstWidth, elapsedTime = 0f;
        while (width > 0)
        {
            elapsedTime += Time.deltaTime;
            width = firstWidth * (1 - (elapsedTime / lifeTIme));
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            SetBoxCollider(width);

            yield return new WaitForFixedUpdate();
        }

        lineRenderer.startWidth = firstWidth;
        lineRenderer.endWidth = firstWidth;
        lineRenderer.enabled = false;
        DestroyBullet();
    }

    private void SetBoxCollider(float width)
    {
        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);
        float lineDistance = (endPos - startPos).magnitude;
        float offsetY = -(lineDistance / 2f);

        boxCollider.offset = new Vector2(0, offsetY);

        boxCollider.size = new Vector2(width, lineDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (collision.CompareTag(targetTag))
            {
                if (collision.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(FinalDamage);
                }
            }
        }
    }
}
