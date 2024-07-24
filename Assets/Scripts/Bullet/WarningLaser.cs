using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WarningLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Default Settings")]
    [SerializeField] private LayerMask blockingLayerMask;
    [SerializeField] private float laserWidth = 0.2f;

    [Header("Luminesce Settings")]
    [SerializeField] private int luminesceCount = 5;
    [SerializeField] private float luminesceDelay = 0.3f;

    private float luminesceTime;
    private Coroutine currentRoutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        luminesceTime = (luminesceCount + 0.5f) * luminesceDelay * 2;
        currentRoutine = null;
    }

    public Vector2 SetPosition(Vector2 origin, Vector2 direction)
    {
        lineRenderer.enabled = true;

        float distance = 100f;

        RaycastHit2D rayHit = Physics2D.Raycast(origin, direction,
            distance, blockingLayerMask);

        Vector2 endPosition = origin + direction * distance;

        if (rayHit)
        {
            endPosition = rayHit.point;
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPosition);

        return endPosition;
    }

    public void SetLaserWidth(float width) 
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public float StartLuminesce()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(Luminesce());
        return luminesceTime;
    } 

    private IEnumerator Luminesce()
    {
        int count = luminesceCount;
        while (count > 0)
        {
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            yield return new WaitForSeconds(luminesceDelay);

            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            yield return new WaitForSeconds(luminesceDelay);

            count--;
        }
    }

    public void StartStretch(Vector2 startPosition, Vector2 endPosition, float stretchSpeed)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(StretchRoutine(startPosition, endPosition, stretchSpeed));
    }

    private IEnumerator StretchRoutine(Vector2 startPosition, Vector2 endPosition, float stretchSpeed)
    {
        lineRenderer.SetPosition(0, startPosition);

        Vector2 direction = (endPosition - startPosition).normalized;

        float distance = (endPosition - startPosition).magnitude;

        float stretchTime = distance / stretchSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < stretchTime)
        {
            lineRenderer.SetPosition(1, startPosition + direction * (distance / stretchTime) * elapsedTime);

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        lineRenderer.SetPosition(1, endPosition);
    }

    public void StopRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
        }
    }

}
