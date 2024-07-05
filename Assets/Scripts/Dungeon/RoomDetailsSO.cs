using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomDetailsSO_", menuName = "Dungeon/RoomDetailsSO")]
public class RoomDetailsSO : ScriptableObject
{
    [SerializeField] private GameObject roomPrefab;

    public RoomType roomType;

    public Doorway[] doorwayArray;

    public Vector2Int[] spawnPositionArray;


}
