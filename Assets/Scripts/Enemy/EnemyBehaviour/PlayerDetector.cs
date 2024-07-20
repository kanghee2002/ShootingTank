using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    [SerializeField] private List<LayerMask> viewObstacleLayerMaskList = new();

    public CircleCollider2D circleCollider;

    private LayerMask compositedObstacleLayerMask;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        compositedObstacleLayerMask = GetObstacleLayerMask();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(Settings.playerTag))
        {
            if (IsTargetVisible(other.transform) && enemyController.IsPlayerDetected == false)
            {
                enemyController.onPlayerDetect?.Invoke(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            enemyController.onPlayerLost?.Invoke();
        }
    }

    public void ExpandDetectRadius(float multiplier)
    {
        if (circleCollider.radius == enemyController.DetectRadius)
        {
            circleCollider.radius = enemyController.DetectRadius * multiplier;
        }
    }

    public void ReduceDetectRadius() => circleCollider.radius = enemyController.DetectRadius;

    private LayerMask GetObstacleLayerMask()
    {
        LayerMask newLayerMask = viewObstacleLayerMaskList[0];
        foreach (var layerMask in viewObstacleLayerMaskList)
        {
            newLayerMask |= layerMask;
        }
        return newLayerMask;
    }

    private bool IsTargetVisible(Transform target)
    {
        var dir = GetTargetDir(target);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position, target.position), compositedObstacleLayerMask);

        //Debug
        if (rayHit) Debug.DrawLine(transform.position, rayHit.point, Color.yellow);
        else Debug.DrawLine(transform.position, target.position, Color.red);

        if (rayHit) return false;
        else return true;
    }

    private Vector3 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;
}
