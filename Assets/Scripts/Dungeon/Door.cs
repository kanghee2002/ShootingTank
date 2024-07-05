using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int connectedPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if Player -> Move Player To Connected Position
    }

    private void MovePlayer(Transform player, Vector2Int movePosition)
    {

    }
}
