using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<LayerMask> viewObstacleMask = new List<LayerMask>();

    private CircleCollider2D circleCollider;

    [SerializeField]
    private float coolTime;
    private bool isCool;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //if (isCool) return;
            Attack(other.transform.position);
        }
    }

    private Vector2 getTargetDir(Vector2 target) 
        => (target - (Vector2)transform.position).normalized;

    protected virtual void Attack(Vector2 target)
    {
        var dir = getTargetDir(target);

    } 
}
