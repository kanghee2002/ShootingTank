using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpPowerBonus;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            if (collision.TryGetComponent(out PlayerController playerController))
            {
                playerController.AddJumpPowerValue(jumpPowerBonus);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController))
        {
            playerController.AddJumpPowerValue(-jumpPowerBonus);
        }
    }
}
