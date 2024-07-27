using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Header("Enemy Prefab")]
    [SerializeField] private List<EnemyPrefabs> enemyPrefabList;

    public bool SpawnEnemy(DungeonLevelSO dungeonLevelSO, DungeonBuilder.RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        Dictionary<RoomType, List<EnemyRank>> compositedEnemyRankDictionary = new();                //ex) Key: Large / Value: [S, A, A, B, B, B, C, C, C, C, C]

        foreach (RoomType regularRoomType in Settings.regularRoomTypeList)
        {
            Dictionary<EnemyRank, int> enemyCountDictionary = dungeonLevelSO.GetEnemyCountDictionary(regularRoomType);

            List<EnemyRank> compositedEnemyRankList = new();

            foreach (KeyValuePair<EnemyRank, int> keyValuePair in enemyCountDictionary)
            {
                for (int i = 0; i < keyValuePair.Value; i++)
                {
                    compositedEnemyRankList.Add(keyValuePair.Key);
                }
            }
            
            compositedEnemyRankDictionary.Add(regularRoomType, compositedEnemyRankList);
        }

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (roomInfos[x, y].roomType == RoomType.Entrance)      //Spawn Player?
                {

                }
                
                if (Settings.regularRoomTypeList.Contains(roomInfos[x, y].roomType))
                {
                    RoomType currentRoomType = roomInfos[x, y].roomType;

                    // Make Clone Enemy Rank List
                    List<EnemyRank> currentCompositedEnemyRankList = compositedEnemyRankDictionary[currentRoomType].ToList();

                    // Get Spawn Position Array
                    Vector2[] enemySpawnPositionArray = roomInfos[x, y].roomDetails.spawnPositionArray;

                    foreach (Vector2 spawnPosition in enemySpawnPositionArray)
                    {
                        // Select Random EnemyRank
                        int randomlySelectedIndex = Random.Range(0, currentCompositedEnemyRankList.Count);
                        EnemyRank selectedEnemyRank = currentCompositedEnemyRankList[randomlySelectedIndex];

                        // Select Enemy Prefab Array By EnemyRank
                        GameObject[] selectedEnemyPrefabArray = null;

                        foreach (EnemyPrefabs enemyPrefabs in enemyPrefabList)
                        {
                            if (enemyPrefabs.enemyRank == selectedEnemyRank)
                            {
                                selectedEnemyPrefabArray = enemyPrefabs.enemyPrefabArray;
                            }
                        }

                        if (selectedEnemyPrefabArray == null)
                        {
                            Debug.Log("Selected Enemy Prefab Array is null");
                            return false;
                        }

                        // Select Random Enemy Prefab in Prefab List
                        GameObject selectedEnemyPrefab = selectedEnemyPrefabArray[Random.Range(0, selectedEnemyPrefabArray.Length)];

                        // Instantiate Enemy Prefab at Spawn Position
                        GameObject instantiatedEnemyObject = Instantiate(selectedEnemyPrefab, roomInfos[x, y].roomTransform);
                        instantiatedEnemyObject.transform.localPosition = spawnPosition;

                        currentCompositedEnemyRankList.RemoveAt(randomlySelectedIndex);
                    }
                }
            }
        }

        return true;
    }

    [System.Serializable]
    private struct EnemyPrefabs
    {
        public EnemyRank enemyRank;
        public GameObject[] enemyPrefabArray;
    }
}
