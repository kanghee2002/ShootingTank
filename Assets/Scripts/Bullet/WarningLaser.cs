using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WarningLaser : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private int luminesceCount = 5;
    private float luminesceDelay = 0.3f;

    private float luminesceTime;

    [SerializeField]
    private float maxDistance;
    public float MaxDistance { get => maxDistance; set => maxDistance = value; }
    [SerializeField]
    private List<LayerMask> blockLayerMasks;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        luminesceTime = luminesceCount * luminesceDelay * 2;


        //Debug
        //StartLuminesce();
    }

    public bool SetPosition(Vector2 startPosition, Vector2 endPosition)
    {
        lineRenderer.enabled = true;

        Vector2 dir = (endPosition - startPosition).normalized;
        float distance = (endPosition - startPosition).magnitude;

        if (distance > maxDistance)
        {
            lineRenderer.enabled = false;
            return false;
        }

        RaycastHit2D rayHit = Physics2D.Raycast(startPosition, dir,
            distance, CombineLayerMasks(blockLayerMasks));
        
        if (rayHit)
        {
            endPosition = rayHit.point;
        }

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        return true;
    }

    public float StartLuminesce()
    {
        StartCoroutine(Luminesce());
        return luminesceTime;
    } 

    private IEnumerator Luminesce()
    {
        int count = luminesceCount;
        float firstWidth = lineRenderer.startWidth;
        while (count > 0)
        {
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            yield return new WaitForSeconds(luminesceDelay);

            lineRenderer.startWidth = firstWidth;
            lineRenderer.endWidth = firstWidth;
            yield return new WaitForSeconds(luminesceDelay);
            count--;
        }
        lineRenderer.enabled = false;
    }

    private LayerMask CombineLayerMasks(List<LayerMask> layerMasks)
    {
        if (layerMasks.Count == 0)
        {
            Debug.Log(layerMasks + " is Empty so cannot combine");
            return 0;
        }

        LayerMask result = layerMasks[0];
        foreach (var layerMask in layerMasks)
        {
            result |= layerMask;
        }
        return result;
    }
}
