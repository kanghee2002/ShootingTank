using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WarningLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField] private int luminesceCount = 5;
    [SerializeField] private float luminesceDelay = 0.3f;
    [SerializeField] private float laserWidth = 0.2f;
    [SerializeField] private LayerMask blockingLayerMask;

    private float luminesceTime;
    private Coroutine currentRoutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        luminesceTime = (luminesceCount + 0.5f) * luminesceDelay * 2;
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

    public float StartLuminesce()
    {
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
