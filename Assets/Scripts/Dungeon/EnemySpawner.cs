using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [System.Serializable]
    private struct EnemyPrefabs
    {
        public int level;
        public EnemyRank enemyRank;
        public GameObject[] enemyPrefabArray;
    }

    [Header("Enemy Prefab")]
    [SerializeField] private List<EnemyPrefabs> enemyPrefabList;

    [SerializeField] private List<Boss> bossList;

    private Dictionary<DungeonBuilder.RoomInfo, List<EnemyController>> spawnedEnemiesDictionary;

    private Boss spawnedBoss;

    public bool SpawnEnemy(DungeonLevelSO dungeonLevelSO, DungeonBuilder.RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        if (spawnedEnemiesDictionary == null )
            spawnedEnemiesDictionary = new();
        else 
            spawnedEnemiesDictionary.Clear();

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
                spawnedEnemiesDictionary[roomInfos[x, y]] = new List<EnemyController>();

                if (Settings.regularRoomTypeList.Contains(roomInfos[x, y].roomType))
                {
                    RoomType currentRoomType = roomInfos[x, y].roomType;

                    // Make Clone Enemy Rank List
                    List<EnemyRank> currentCompositedEnemyRankList = compositedEnemyRankDictionary[currentRoomType].ToList();

                    // Get Spawn Position List
                    List<Vector2> enemySpawnPositionList = roomInfos[x, y].roomDetails.spawnPositionArray.ToList();

                    while (currentCompositedEnemyRankList.Count > 0)
                    {
                        // Select Spawn Position
                        if (enemySpawnPositionList.Count <= 0) Debug.Log("Error: SpawnPosition Count is less than Enemy Count");
                        int randomlySelectedIndex = Random.Range(0, enemySpawnPositionList.Count);
                        Vector2 spawnPosition = enemySpawnPositionList[randomlySelectedIndex];
                        enemySpawnPositionList.RemoveAt(randomlySelectedIndex);


                        // Select Random EnemyRank
                        randomlySelectedIndex = Random.Range(0, currentCompositedEnemyRankList.Count);
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

                        EnemyController enemyController = instantiatedEnemyObject.GetComponent<EnemyController>();
                        spawnedEnemiesDictionary[roomInfos[x, y]].Add(enemyController);
                        enemyController.SetActiveEnemyController(false);

                        Enemy enemy = instantiatedEnemyObject.GetComponent<Enemy>();
                        enemy.currentLevel = dungeonLevelSO.level;

                        currentCompositedEnemyRankList.RemoveAt(randomlySelectedIndex);
                    }
                }
                if (roomInfos[x, y].roomType == RoomType.Boss)
                {
                    List<Boss> selectedBossList = bossList.Where(boss => boss.SpawnLevel == dungeonLevelSO.level).ToList();

                    Boss randomBoss = selectedBossList[Random.Range(0, selectedBossList.Count)];

                    Vector2 spawnPosition = (Vector2)roomInfos[x, y].roomTransform.position + roomInfos[x, y].roomDetails.spawnPositionArray[0];

                    spawnedBoss = Instantiate(randomBoss, spawnPosition, Quaternion.identity);
                }
            }
        }

        return true;
    }

    public void SetActiveEnemiesInRoom(Transform roomTransform, bool isActive, bool isBoss)
    {
        if (!isBoss)
        {
            foreach (KeyValuePair<DungeonBuilder.RoomInfo, List<EnemyController>> keyValuePair in spawnedEnemiesDictionary)
            {
                DungeonBuilder.RoomInfo currentRoomInfo = keyValuePair.Key;
                List<EnemyController> enemyList = keyValuePair.Value;

                if (currentRoomInfo.roomTransform == roomTransform)
                {
                    foreach (EnemyController enemyController in enemyList)
                    {
                        enemyController.SetActiveEnemyController(isActive);
                    }
                }
            }
        }
        else
        {
            if (isActive)
            {
                spawnedBoss.Initialize(GameManager.Instance.playerObject.transform);
            }
        }
    }
}
