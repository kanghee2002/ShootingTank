using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("ENEMY SPAWN VALUES")]
    [SerializeField]
    private List<EnemyCountPerRoomType> enemyCountList = new();

    [Header("ROOM DETAILS ARRAY")]
    public RoomDetailsSO[] roomDetailsArray;

    [Header("DOOR PREFABS")]
    public GameObject verticalDoorPrefab;
    public GameObject horizontalDoorPrefab;

    public Dictionary<EnemyRank, int> GetEnemyCountDictionary(RoomType roomType)
    {
        /*if (enemyCountList.Count != System.Enum.GetValues(typeof(EnemyRank)).Length)
        {
            Debug.Log(this.name + "'s Enemy Count List is Uncompleted");
            return null;
        }*/

        Dictionary<EnemyRank, int> enemyCountDictionary = new();

        foreach (EnemyCountPerRoomType enemyCountPerRoomType in enemyCountList)
        {
            if (enemyCountPerRoomType.roomType == roomType)
            {
                foreach (EnemyCount enemyCount in enemyCountPerRoomType.enemyCount)
                {
                    enemyCountDictionary.Add(enemyCount.enemyRank, enemyCount.count);
                }
            }
        }

        return enemyCountDictionary;
    }

    [System.Serializable]
    private struct EnemyCountPerRoomType
    {
        public RoomType roomType;
        public EnemyCount[] enemyCount;
    }


    [System.Serializable]
    private struct EnemyCount
    {
        public EnemyRank enemyRank;
        public int count;
    }
}
