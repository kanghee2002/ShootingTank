using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevelSO_", menuName = "Dungeon/DungeonLevelSO")]
public class DungeonLevelSO : ScriptableObject
{
    public int level;

    public int dungeonWidth;
    public int dungeonHeight;

    public int horizontallyBlockedDoorCountPerFloor;
    public int verticallyConnectedDoorCountPerFloor;

    public int roomGap;

    public int chestCount;
    public int shopCount;

    public RoomDetailsSO[] roomDetails;
}
