using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<LayerMask> viewObstacleMask = new List<LayerMask>();

    private CircleCollider2D circleCollider;

    private Transform player;

    [SerializeField]
    private float coolTime;
    private bool isCool;
    private bool isPlayerDetected;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        isCool = false;
        isPlayerDetected = false;
    }

    private void Update()
    {
        if (isAttackPossible()) Attack(player);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsTargetVisible(other.transform) && isPlayerDetected == false)
            {
                isPlayerDetected = true;
                player = other.transform;
            }
        }
    }

    private bool IsTargetVisible(Transform target)
    {
        Vector2 dir = GetTargetDir(target);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, dir,
                                                circleCollider.radius, GetObstacleLayerMask());
        
        //Debug
        if (rayHit) Debug.DrawLine(transform.position, rayHit.point, Color.yellow);
        else Debug.DrawLine(transform.position, target.position, Color.red);

        if (rayHit) return false;
        else return true;
    }

    private bool isAttackPossible() => (isPlayerDetected && !isCool);

    protected virtual void Attack(Transform target)
    {
        var dir = GetTargetDir(target);
        //Implement Attack
    }

    private Vector2 GetTargetDir(Transform target)
        => (target.position - transform.position).normalized;

    public LayerMask GetObstacleLayerMask()
    {
        LayerMask newLayerMask = viewObstacleMask[0];
        foreach (var layerMask in viewObstacleMask)
        {
            newLayerMask = newLayerMask | layerMask;
        }
        return newLayerMask;
    }
}
