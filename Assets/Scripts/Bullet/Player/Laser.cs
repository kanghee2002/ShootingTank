using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Bullet
{
    private LineRenderer lineRenderer;
    private PolygonCollider2D polygonCollider;

    private float firstWidth;

    [SerializeField]
    private List<string> blockLayerMasks;

    [SerializeField]
    private float distance;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        firstWidth = lineRenderer.startWidth;
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    public void Activate(Vector3 dir)
    {
        Vector3 startPosition = transform.position, endPosition;

        RaycastHit2D rayHit = Physics2D.Raycast(startPosition, dir, 
            (startPosition + dir * distance).magnitude, LayerMask.GetMask(blockLayerMasks.ToArray()));

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

        StartCoroutine(FadeOut());
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

    public bool AddBlockLayerMask(string layerMask)
    {
        if (blockLayerMasks.Contains(layerMask))
        {
            return false;
        }
        else
        {
            blockLayerMasks.Add(layerMask);
            return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var targetTag in targetTags)
        {
            if (collision.CompareTag(targetTag))
            {
                if (collision.TryGetComponent(out Health health))
                {
                    health.TakeDamage(FinalDamage);
                }
            }
        }
    }
}
