using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 connectedPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag) || collision.CompareTag(Settings.enemyTag))
        {
            MoveObject(collision.transform, connectedPosition);
        }
    }

    private void MoveObject(Transform player, Vector3 movePosition)
    {
        player.transform.position = movePosition;
    }
}
