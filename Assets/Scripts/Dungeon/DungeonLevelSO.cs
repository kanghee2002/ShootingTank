using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelSO_", menuName = "Dungeon/DungeonLevelSO")]
public class DungeonLevelSO : ScriptableObject
{
    [Header("DEFAULT VALUES")]
    public int level;

    public int dungeonWidth;
    public int dungeonHeight;

    public int horizontallyBlockedDoorCountPerFloor;
    public int verticallyConnectedDoorCountPerFloor;

    public int roomGap;

    [Header("ROOM COUNT VALUES")]
    public int chestRoomCount;
    public int shopRoomCount;
    public int hiddenRoomCount;

    [Header("ROOM PERCENTAGE VALUES")]
    public float visitableRoomPercentageMin;

    public float smallRoomPercentage;
    public float mediumRoomPercentage;
    public float largeRoomPercentage;

    [Header("ROOM DETAILS ARRAY")]
    public RoomDetailsSO[] roomDetailsArray;

    [Header("DOOR PREFABS")]
    public GameObject verticalDoorPrefab;
    public GameObject horizontalDoorPrefab;

}
