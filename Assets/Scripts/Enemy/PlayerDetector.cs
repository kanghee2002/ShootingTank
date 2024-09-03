using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Parent Reference")]
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private UtilityObject utilityObject;
    [SerializeField] private Coin coinObject;

    [SerializeField] private LayerMask viewObstacleLayerMask;

    [HideInInspector] public CircleCollider2D circleCollider;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (IsTargetVisible(collision.transform))
            {
                if (utilityObject != null)
                {
                    utilityObject.OnDetectPlayer(collision.transform);
                }
                if (coinObject != null)
                {
                    coinObject.OnDetectPlayer(collision.transform);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (IsTargetVisible(collision.transform))
            {
                if (enemyController != null)
                {
                    if (enemyController.IsPlayerDetected == false)
                    {
                        enemyController.onPlayerDetect?.Invoke(collision.transform);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (enemyController != null)
            {
                enemyController.onPlayerLost?.Invoke();
            }
        }
    }

    public void ExpandDetectRadius(float multiplier)
    {
        if (enemyController != null)
        {
            if (circleCollider.radius == enemyController.DetectRadius)
            {
                circleCollider.radius = enemyController.DetectRadius * multiplier;
            }
        }
    }

    public void ReduceDetectRadius()
    {
        if (enemyController != null)
        {
            circleCollider.radius = enemyController.DetectRadius;
        }
    }

    private bool IsTargetVisible(Transform target)
    {
        var dir = GetTargetDir(target);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position, target.position), viewObstacleLayerMask);

        //Debug
        if (rayHit) Debug.DrawLine(transform.position, rayHit.point, Color.yellow);
        else Debug.DrawLine(transform.position, target.position, Color.red);

        if (rayHit) return false;
        else return true;
    }

    private Vector3 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;
}
