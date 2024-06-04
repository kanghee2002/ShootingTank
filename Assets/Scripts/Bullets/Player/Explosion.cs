using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Transform parent;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private string targetTag;
    public string TargetTag { get => targetTag; set => targetTag = value; }

    [SerializeField]
    private float damageAmount;
    public float DamageAmount { get => damageAmount; set => damageAmount = value; }

    [SerializeField]
    private float radius;
    public float Radius { get => radius; set => radius = value; }

    [SerializeField]
    private float explosionTime;

    private void Awake()
    {
        parent = transform.parent;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(Explode());
        StartCoroutine(FadeOut());
    }

    private IEnumerator Explode()
    {
        var targets = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask(targetTag));
        foreach (var target in targets)
        {
            if (target.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damageAmount);
            }
        }
        yield return new WaitForSeconds(explosionTime);
        gameObject.SetActive(false);
        if (parent != null) transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < explosionTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f - (elapsedTime / explosionTime));

            elapsedTime += Time.deltaTime;        
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
