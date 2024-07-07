using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonBuilder : Singleton<DungeonBuilder>
{
    [SerializeField] private DungeonLevelSO[] dungeonLevelSOArray;

    // nn 퍼센트도 못다니면 다시 만들기

    #region DEBUG
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject blockRed;
    [SerializeField] private GameObject blockBlue;
    #endregion DEBUG

    private struct RoomInfo
    {
        public RoomDetailsSO roomDetails;
        public bool isUpConnected;
        public bool isDownConnected;
        public bool isLeftConnected;
        public bool isRightConnected;
        public RoomType roomType;
    }

    private void Start()
    {
        BuildDungeon(1);
    }

    public void BuildDungeon(int level)
    {
        DungeonLevelSO selectedDungeonLevel = SelectDungeonLevelSO(level);

        int buildAttemptCount = 10000;

        RoomInfo[,] completedRoomInfos;

        bool isSuccess = AttemptToBuildDungeon(selectedDungeonLevel, buildAttemptCount, out completedRoomInfos);

        if (isSuccess)
        {
            InstantiatedRooms(selectedDungeonLevel, completedRoomInfos);

            /* // DEBUG
            int smallCount = 0;
            int mediumCount = 0;
            int largeCount = 0;
            foreach (var room in completedRoomInfos)
            {
                if (room.roomType == RoomType.Small) smallCount++;
                else if (room.roomType == RoomType.Medium) mediumCount++;
                else if (room.roomType == RoomType.Large) largeCount++;
            }
            Debug.Log("Small Room Count : " + smallCount);
            Debug.Log("Medium Room Count : " + mediumCount);
            Debug.Log("Large Room Count : " + largeCount);
            // DEBUG */

            // Set Door Values
        }
        else
        {
            Debug.Log("Unable to Build Dungeon");
        }

    }

    private bool AttemptToBuildDungeon(DungeonLevelSO selectedDungeonLevel, int maxLoopCount, out RoomInfo[,] roomInfos)
    {
        int dungeonWidth = selectedDungeonLevel.dungeonWidth;
        int dungeonHeight = selectedDungeonLevel.dungeonHeight;

        roomInfos = new RoomInfo[dungeonWidth, dungeonHeight];

        bool isBuildSuccess = false;
        int loopCount = 0;
        
        while (loopCount < maxLoopCount)
        {
            InitializeDungeon(roomInfos);

            SetDefaultRoomSettings(roomInfos, selectedDungeonLevel.roomDetailsArray);

            SetDoors(roomInfos, selectedDungeonLevel.horizontallyBlockedDoorCountPerFloor, selectedDungeonLevel.verticallyConnectedDoorCountPerFloor);

            int[,] distanceArray = BFS(roomInfos, new Vector2Int(0, dungeonHeight - 1), new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.down });

            int visitableRoomCount = CountVisitableRooms(distanceArray);

            float visitableRoomPercentage = (float)visitableRoomCount / (dungeonWidth * dungeonHeight) * 100f;

            if (visitableRoomPercentage >= selectedDungeonLevel.visitableRoomPercentageMin)
            {
                SetRoomTypes(selectedDungeonLevel, roomInfos);

                SetRoomDetails(selectedDungeonLevel, roomInfos);

                InstantiatedRooms(selectedDungeonLevel, roomInfos);

                isBuildSuccess = true;
                break;
            }

            loopCount++;
        }

        return isBuildSuccess;
    }

    private DungeonLevelSO SelectDungeonLevelSO(int level)
    {
        foreach (DungeonLevelSO dungeonLevelSO in dungeonLevelSOArray)
        {
            if (dungeonLevelSO.level == level)
            {
                return dungeonLevelSO;
            }
        }

        Debug.Log("Dungeon Builder doesn't have level '" + level + "' dungeonlevelSO");
        return null;
    }

    private void InitializeDungeon(RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                roomInfos[x, y].roomDetails = null;
                roomInfos[x, y].isUpConnected = false;
                roomInfos[x, y].isDownConnected = false;
                roomInfos[x, y].isLeftConnected = true;
                roomInfos[x, y].isRightConnected = true;
                if (x == 0)
                {
                    roomInfos[x, y].isLeftConnected = false;
                }
                if (x == dungeonWidth - 1)
                {
                    roomInfos[x, y].isRightConnected = false;
                }

                roomInfos[x, y].roomType = RoomType.None;
            }
        }
    }

    private void SetDefaultRoomSettings(RoomInfo[,] roomInfos, RoomDetailsSO[] roomDetailsArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        // Set Entrance Room
        roomInfos[0, dungeonHeight - 1].roomType = RoomType.Entrance;
        List<RoomDetailsSO> entranceRoomDetailsList = GetSpecificRoomDetails(roomDetailsArray, RoomType.Entrance);
        roomInfos[0, dungeonHeight - 1].roomDetails = entranceRoomDetailsList[Random.Range(0, entranceRoomDetailsList.Count)];

        // Set Boss Room
        roomInfos[dungeonWidth - 1, 0].roomType = RoomType.Boss;
        List<RoomDetailsSO> bossRoomDetailsList = GetSpecificRoomDetails(roomDetailsArray, RoomType.Boss);
        roomInfos[dungeonWidth - 1, 0].roomDetails = bossRoomDetailsList[Random.Range(0, bossRoomDetailsList.Count)];
    }

    private void SetDoors(RoomInfo[,] roomInfos, int horizontallyBlockedDoorCountPerFloor, int verticallyConnectedDoorCountPerFloor)
    {
        int dungeonHeight = roomInfos.GetLength(1);

        for (int row = 1; row < dungeonHeight; row++)
        {
            SetHorizontallyBlockedDoors(roomInfos, horizontallyBlockedDoorCountPerFloor, row);
            SetVerticallyConnectedDoors(roomInfos, verticallyConnectedDoorCountPerFloor, row);
        }
    }

    private void SetHorizontallyBlockedDoors(RoomInfo[,] roomInfos, int blockedDoorCount, int row)
    {
        int dungeonWidth = roomInfos.GetLength(0);

        List<int> selectedColumnsList = new();

        for (int i = 0; i < blockedDoorCount; i++)
        {
            int selectedColumn = Random.Range(0, dungeonWidth - 1);
            selectedColumnsList.Add(selectedColumn);
        }

        foreach (int col in selectedColumnsList)
        {
            roomInfos[col, row].isRightConnected = false;
            roomInfos[col + 1, row].isLeftConnected = false;

            //Debug.Log("(" + col + ", " + row + ")'s Right is Blocked");
        }
    }

    private void SetVerticallyConnectedDoors(RoomInfo[,] roomInfos, int blockedDoorCount, int row)
    {
        int dungeonWidth = roomInfos.GetLength(0);

        List<int> leftBlockedRoomColumnList = new();

        for(int i = 1; i < dungeonWidth; i++)
        {
            if (!roomInfos[i, row].isLeftConnected)
            {
                leftBlockedRoomColumnList.Add(i);
            }
        }
        leftBlockedRoomColumnList.Add(dungeonWidth);

        List<int> selectedColumnList = new();

        int previousColumn = 0;
        foreach (int col in leftBlockedRoomColumnList)
        {
            for (int i = 0; i < blockedDoorCount; i++)
            {

                int selectedColumn = Random.Range(previousColumn, col);
                selectedColumnList.Add(selectedColumn);

                //Debug.Log("Selected Column : " + selectedColumn + ", Range : " + previousColumn + " / " + col);
            }

            previousColumn = col;
        }

        foreach (int col in selectedColumnList)
        {
            //Debug.Log("(" + col + ", " + row + ")'s Down is Connected");
            roomInfos[col, row].isDownConnected = true;
            roomInfos[col, row - 1].isUpConnected = true;

        }
    }

    private List<RoomDetailsSO> GetSpecificRoomDetails(RoomDetailsSO[] roomDetailsArray, RoomType roomType)
    {
        List<RoomDetailsSO> resultRoomDetailsList = new();

        foreach (RoomDetailsSO roomDetails in roomDetailsArray)
        {
            if (roomDetails.roomType == roomType)
            {
                resultRoomDetailsList.Add(roomDetails);
            }
        }

        return resultRoomDetailsList;
    }

    private int CountVisitableRooms(int[,] distanceArray)
    {
        int dungeonWidth = distanceArray.GetLength(0);
        int dungeonHeight = distanceArray.GetLength(1);

        int visitableRoomCount = 0;

        for(int i = 0; i < dungeonWidth;i++)
        {
            for (int j = 0; j < dungeonHeight;j++)
            {
                if (distanceArray[i, j] >= 0)
                {
                    visitableRoomCount++;
                }
            }
        }
        return visitableRoomCount;
    }

    private void SetRoomTypes(DungeonLevelSO dungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        int totalRoomCount = dungeonWidth * dungeonHeight;
        int untypedRoomCount = totalRoomCount - 2;         // Entrance and Boss

        Dictionary<RoomType, int> roomCountDictionary = new()
        {
            { RoomType.Small, GetRoomCountByPercentage(dungeonLevel.smallRoomPercentage, totalRoomCount) },
            { RoomType.Medium, GetRoomCountByPercentage(dungeonLevel.mediumRoomPercentage, totalRoomCount) },
            { RoomType.Large, GetRoomCountByPercentage(dungeonLevel.largeRoomPercentage, totalRoomCount) }
        };
        // NEED TO ADD CHEST, SHOP, HIDDEN

        int loopCount = 0;
        bool isDone = false;

        while (loopCount < 10000)
        {
            int regularRoomCount = 0;
            foreach (KeyValuePair<RoomType, int> keyValuePair in roomCountDictionary)
            {
                regularRoomCount += keyValuePair.Value;
            }
            if (regularRoomCount >= untypedRoomCount)
            {
                isDone = true;
                break;
            }

            RoomType randomRoomType = GetRandomRegularRoomType();

            roomCountDictionary[randomRoomType]++;
            loopCount++;
        }

        if (!isDone)
        {
            Debug.Log("Unable to Set Regular Room Types");
        }

        /* DEBUG
        foreach (KeyValuePair<RoomType, int> keyValuePair in roomCountDictionary)
        {
            Debug.Log(keyValuePair.Key + " Count : " + keyValuePair.Value);
        }
        */

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                if (roomInfos[i, j].roomType == RoomType.Entrance || roomInfos[i, j].roomType == RoomType.Boss) 
                {
                    continue;
                }

                loopCount = 0;
                isDone = false;

                while (loopCount < 10000)
                {
                    RoomType selectedRoomType = GetRandomRegularRoomType();

                    if (roomCountDictionary[selectedRoomType] > 0)
                    {
                        roomInfos[i, j].roomType = selectedRoomType;
                        roomCountDictionary[selectedRoomType]--;
                        isDone = true;
                        break;
                    }

                    loopCount++;
                }

                if (!isDone) Debug.Log("Unable to select Room type");
            }
        }
    }

    private void SetRoomDetails(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                List<RoomDetailsSO> selectedRoomDetailsList = GetSpecificRoomDetails(selectedDungeonLevel.roomDetailsArray, roomInfos[i, j].roomType);
                roomInfos[i, j].roomDetails = selectedRoomDetailsList[Random.Range(0, selectedRoomDetailsList.Count)];
            }
        }

    }

    private int GetRoomCountByPercentage(float roomPercentage, int totalRoomCount) => Mathf.FloorToInt(roomPercentage * (float)totalRoomCount / 100f);

    private RoomType GetRandomRegularRoomType()
    {
        int regularRoomCount = Random.Range(0, 3);
        if (regularRoomCount == 0)
            return RoomType.Small;
        else if (regularRoomCount == 1)
            return RoomType.Medium;
        else if (regularRoomCount == 2)
            return RoomType.Large;
        else
            return RoomType.None;
    }

    private void SetHiddenRoom(RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        int[,] distanceArray = BFS(roomInfos, new Vector2Int(0, dungeonHeight - 1), new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right });

        int maxDistance = 0;
        Vector2Int farthestPosition = Vector2Int.zero;

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                if (distanceArray[i, j] > maxDistance)
                {
                    maxDistance = distanceArray[i, j];
                    farthestPosition = new Vector2Int(i, j);
                }
            }
        }

        roomInfos[farthestPosition.x, farthestPosition.y].roomType = RoomType.Hidden;
    }

    private void InstantiatedRooms(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                GameObject roomGameObject = Instantiate(roomInfos[x, y].roomDetails.roomPrefab, this.transform);
                roomGameObject.transform.localPosition = new Vector3(x * selectedDungeonLevel.roomGap, y * selectedDungeonLevel.roomGap, 0f);
            }
        }
    }

    private int[,] BFS(RoomInfo[,] roomInfos, Vector2Int startPosition, List<Vector2Int> directions)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        Queue<Vector2Int> queue = new();
        int[,] visited = new int[dungeonWidth, dungeonHeight];

        for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                visited[i, j] = -1;
            }
        }

        queue.Enqueue(startPosition);
        visited[startPosition.x, startPosition.y] = 0;

        while (queue.Count > 0)
        {
            Vector2Int currentPosition = queue.Dequeue();

            foreach (Vector2Int direction in directions)
            {
                
                Vector2Int nextPosition = currentPosition + direction;
                if (nextPosition.x < 0 || nextPosition.x >= dungeonWidth || nextPosition.y < 0 || nextPosition.y >= dungeonHeight)
                    continue;

                if (visited[nextPosition.x, nextPosition.y] != -1)
                    continue;

                RoomInfo currentRoomInfo = roomInfos[currentPosition.x, currentPosition.y];

                if ((direction == Vector2Int.up && !currentRoomInfo.isUpConnected) ||
                    (direction == Vector2Int.down && !currentRoomInfo.isDownConnected) ||
                    (direction == Vector2Int.left && !currentRoomInfo.isLeftConnected) ||
                    (direction == Vector2Int.right && !currentRoomInfo.isRightConnected))
                    continue;

                if (visited[nextPosition.x, nextPosition.y] == -1)
                {
                    visited[nextPosition.x, nextPosition.y] = visited[currentPosition.x, currentPosition.y] + 1;
                    queue.Enqueue(nextPosition);
                }
            }
        }

        //DEBUG
        /*for (int i = 0; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                Debug.Log(i + ", " + j + " : " + visited[i, j]);
            }
        }*/

        return visited;
    }


    #region DEBUG
    private void PrintRoomInfo(RoomInfo[,] roomInfos, int[,] distanceArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);
        for (int i = 0 ; i < dungeonWidth; i++)
        {
            for (int j = 0; j < dungeonHeight; j++)
            {
                int realPosX = i * 2 + 1;
                int realPosY = j * 2 + 1;

                if (!roomInfos[i, j].isDownConnected)
                {
                    Instantiate(block, new Vector3(realPosX, realPosY - 1, 0), Quaternion.identity);
                }
                if (!roomInfos[i, j].isUpConnected)
                {
                    Instantiate(block, new Vector3(realPosX, realPosY + 1, 0), Quaternion.identity);
                }
                if (!roomInfos[i, j].isLeftConnected)
                {
                    Instantiate(block, new Vector3(realPosX - 1, realPosY, 0), Quaternion.identity);
                }
                if (!roomInfos[i, j].isRightConnected)
                {
                    Instantiate(block, new Vector3(realPosX + 1, realPosY, 0), Quaternion.identity);
                }
                Instantiate(block, new Vector3(realPosX - 1, realPosY - 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX - 1, realPosY + 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX + 1, realPosY - 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX + 1, realPosY + 1, 0), Quaternion.identity);
            
                if (distanceArray[i, j] >= 0)
                {
                    Instantiate(blockBlue, new Vector3(realPosX, realPosY, 0), Quaternion.identity);
                }
                else
                {
                    Instantiate(blockRed, new Vector3(realPosX, realPosY, 0), Quaternion.identity);

                }
            }
        }
    }

    #endregion DEBUG
}
