using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private int value;

    private Rigidbody2D rigid;
    private Collider2D myCollider;

    private bool isPlayerDetected;
    private Transform playerTransform;
    private float maxDistance;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        isPlayerDetected = false;
        playerTransform = null;
        maxDistance = 10f;
    }

    private void Update()
    {
        if (isPlayerDetected)
        {
            MoveToPlayer();
        }
    }

    public void OnDetectPlayer(Transform playerTransform)
    {
        isPlayerDetected = true;
        this.playerTransform = playerTransform;

        myCollider.isTrigger = true;
    }

    private void OnLostPlayer()
    {
        isPlayerDetected = false;
        playerTransform = null;

        myCollider.isTrigger = false;
    }

    private void MoveToPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        rigid.velocity = direction * moveSpeed;

        if (Vector3.Distance(playerTransform.position, transform.position) > maxDistance)
        {
            OnLostPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (collision.TryGetComponent(out PlayerData playerData))
            {
                playerData.GetCoin(value);

                gameObject.SetActive(false);
            }
        }
    }
}
