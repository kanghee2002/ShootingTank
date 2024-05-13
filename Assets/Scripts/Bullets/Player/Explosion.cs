using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private GameObject missileObj;

    [SerializeField]
    private string targetTag;

    [SerializeField]
    private float damageAmount;
    public float DamageAmount { get => damageAmount; set => damageAmount = value; }

    [SerializeField]
    private float radius;
    public float Radius { get => radius; set => radius = value; }

    private void Awake()
    {
        missileObj = transform.parent.gameObject;
    }

    public IEnumerator Explode()
    {
        var targets = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask(targetTag));
        Debug.Log("Explosion Detect " + targets.Length + " enemies");
        foreach (var target in targets)
        {
            Debug.Log("Enemy : " + target);
            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damageAmount);
                Debug.Log("Damage " + target + " : " + damageAmount);
            }
        }
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
        transform.parent = missileObj.transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
