using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 connectedPosition;
    public Transform connectedRoomTransform;
    public RoomType connectedRoomType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag) || collision.CompareTag(Settings.enemyTag))
        {
            MoveObject(collision.transform, connectedPosition);
        }
    }

    private void MoveObject(Transform objectTransform, Vector3 movePosition)
    {
        objectTransform.position = movePosition;
        StageManager.Instance.OnEnterRoom(connectedRoomTransform.gameObject, connectedRoomType);
    }
}
