using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    [SerializeField] private List<LayerMask> viewObstacleMaskList = new();

    private CircleCollider2D circleCollider;
    private LayerMask compositedObstacleMask;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        compositedObstacleMask = GetObstacleLayerMask();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag))
        {
            if (IsTargetVisible(other.transform) && enemyController.IsPlayerDetected == false)
            {
                enemyController.OnPlayerDetected(other.transform);
            }
        }
    }

    private LayerMask GetObstacleLayerMask()
    {
        LayerMask newLayerMask = viewObstacleMaskList[0];
        foreach (var layerMask in viewObstacleMaskList)
        {
            newLayerMask |= layerMask;
        }
        return newLayerMask;
    }

    private bool IsTargetVisible(Transform target)
    {
        var dir = GetTargetDir(target);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir,
                                                circleCollider.radius, compositedObstacleMask);

        //Debug
        if (rayHit) Debug.DrawLine(transform.position, rayHit.point, Color.yellow);
        else Debug.DrawLine(transform.position, target.position, Color.red);

        if (rayHit) return false;
        else return true;
    }

    private Vector3 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;
}
