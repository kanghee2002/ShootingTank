using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UtilityObject : MonoBehaviour
{
    // Child Object's Trigger Collider detects player
    // if child detects, this flies to player
    // if this detects player, then add playerUtilty to player's child (Utilities) and destroy itself
    // When it fly, it doesn't collide with platform or others

    [Header("Utility Reference")]
    [SerializeField] private Transform utilityTransform;

    [Header("Default Settings")]
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rigid;
    private Collider2D myCollider;
    private PlayerUtility playerUtility;

    private bool isPlayerDetected;
    private Transform playerTransform;
    private float maxDistance;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        playerUtility = utilityTransform.GetComponent<PlayerUtility>();
        
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
            if (collision.TryGetComponent(out PlayerController playerController))
            {
                utilityTransform.SetParent(playerController.utilityAnchorTransform);

                playerUtility.playerTransform = collision.transform;

                utilityTransform.gameObject.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }
}
