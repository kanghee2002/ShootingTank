using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Transform parent;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private List<string> targetLayerMasks;

    [SerializeField]
    private float damageAmount;
    public float DamageAmount { get => damageAmount; set => damageAmount = value; }

    [SerializeField] private float radius;
    public float Radius { get => radius; set => radius = value; }

    [SerializeField] private float explosionTime;

    private float elapsedTime = 0f;

    private void Awake()
    {
        parent = transform.parent;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(Explode());

        elapsedTime = 0f;
    }

    private void Update()
    {
        FadeOut();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Explode()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask(targetLayerMasks.ToArray()));
            
        foreach (Collider2D target in targets)
        {
            if (target.TryGetComponent(out Health health))
            {
                health.TakeDamage(damageAmount);
            }
        }
        yield return new WaitForSeconds(explosionTime);
        gameObject.SetActive(false);
        if (parent != null) transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    private void FadeOut()
    {
        if (elapsedTime < explosionTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f - (elapsedTime / explosionTime));

            elapsedTime += Time.deltaTime;
        }
    }

    public bool AddTargetLayerMask(string layerMask)
    {
        if (targetLayerMasks.Contains(layerMask))
        {
            return false;
        }
        else
        {
            targetLayerMasks.Add(layerMask);
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
