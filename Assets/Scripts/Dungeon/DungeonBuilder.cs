﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonBuilder : Singleton<DungeonBuilder>
{
    [Header("Dungeon Level SO")]
    [SerializeField] private DungeonLevelSO[] dungeonLevelSOArray;

    #region DEBUG
    [Header("DEBUG")]
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject blockRed;
    [SerializeField] private GameObject blockBlue;
    #endregion DEBUG

 
    public struct RoomInfo
    {
        public RoomDetailsSO roomDetails;
        public bool isUpConnected;
        public bool isDownConnected;
        public bool isLeftConnected;
        public bool isRightConnected;
        public RoomType roomType;
        public Transform roomTransform;
    }

    private RoomInfo[,] currentRoomInfos;

    [Header("Boss Room Blocking")]
    [SerializeField] private GameObject horizontalPlayerBlockingPrefab;
    [SerializeField] private GameObject verticalPlayerBlockingPrefab;
    private RoomInfo bossRoomInfo;
    public RoomInfo GetBossRoomInfo() => bossRoomInfo;
    private List<Door> bossRoomDoorList;
    private List<GameObject> instantiatedBlockingList;

    [Header("Shop Prefab")]
    [SerializeField] private GameObject shopPrefab;

    protected override void Awake()
    {
        currentRoomInfos = null;
        bossRoomDoorList = new();
        instantiatedBlockingList = new();
    }

    private void Start()
    {
        Vector2 playerPosition = BuildDungeon(GameManager.Instance.CurrentDungeonLevel);
        GameManager.Instance.playerObject.transform.position = playerPosition;
    }

    private Vector2 BuildDungeon(int level)
    {
        DungeonLevelSO selectedDungeonLevel = SelectDungeonLevelSO(level);

        int buildAttemptCount = 10000;

        bool isSuccess = AttemptToBuildDungeon(selectedDungeonLevel, buildAttemptCount, out currentRoomInfos);

        if (isSuccess)
        {
            InstantiateRooms(selectedDungeonLevel, currentRoomInfos);

            InstantiateDoors(selectedDungeonLevel, currentRoomInfos);

            BlockUnusedDoors(currentRoomInfos);

            //InstantiateShops(currentRoomInfos);       Shop is in Room Prefab

            #region DEBUG
            /*
            int smallCount = 0;
            int mediumCount = 0;
            int largeCount = 0;
            foreach (var room in currentRoomInfos)
            {
                if (room.roomType == RoomType.Small) smallCount++;
                else if (room.roomType == RoomType.Medium) mediumCount++;
                else if (room.roomType == RoomType.Large) largeCount++;
            }
            Debug.Log("Small Room Count : " + smallCount);
            Debug.Log("Medium Room Count : " + mediumCount);
            Debug.Log("Large Room Count : " + largeCount);
            // DEBUG */
            #endregion DEBUG

            EnemySpawner.Instance.SpawnEnemy(selectedDungeonLevel, currentRoomInfos);

            bossRoomInfo = currentRoomInfos[selectedDungeonLevel.dungeonWidth - 1, 0];

            RoomInfo entranceRoomInfo = currentRoomInfos[0, selectedDungeonLevel.dungeonHeight - 1];

            StageManager.Instance.currentRoomTransform = entranceRoomInfo.roomTransform;

            //entranceRoomInfo.roomTransform.gameObject.SetActive(true);

            return (Vector2)entranceRoomInfo.roomTransform.position + entranceRoomInfo.roomDetails.spawnPositionArray[0];
        }
        else
        {
            Debug.Log("Unable to Build Dungeon");

            return Vector2.zero;
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
                SetRoomTypes(selectedDungeonLevel, roomInfos, distanceArray);

                SetRoomDetails(selectedDungeonLevel, roomInfos);

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

        for (int x = 0; x < blockedDoorCount; x++)
        {
            int selectedColumn = Random.Range(0, dungeonWidth - 1);
            selectedColumnsList.Add(selectedColumn);
        }

        foreach (int col in selectedColumnsList)
        {
            roomInfos[col, row].isRightConnected = false;
            roomInfos[col + 1, row].isLeftConnected = false;
        }
    }

    private void SetVerticallyConnectedDoors(RoomInfo[,] roomInfos, int blockedDoorCount, int row)
    {
        int dungeonWidth = roomInfos.GetLength(0);

        List<int> leftBlockedRoomColumnList = new();

        for(int x = 1; x < dungeonWidth; x++)
        {
            if (!roomInfos[x, row].isLeftConnected)
            {
                leftBlockedRoomColumnList.Add(x);
            }
        }
        leftBlockedRoomColumnList.Add(dungeonWidth);

        List<int> selectedColumnList = new();

        int previousColumn = 0;
        foreach (int col in leftBlockedRoomColumnList)
        {
            for (int x = 0; x < blockedDoorCount; x++)
            {
                int selectedColumn = Random.Range(previousColumn, col);
                selectedColumnList.Add(selectedColumn);
            }

            previousColumn = col;
        }

        foreach (int col in selectedColumnList)
        {
            roomInfos[col, row].isDownConnected = true;
            roomInfos[col, row - 1].isUpConnected = true;
        }
    }

    private int CountVisitableRooms(int[,] distanceArray)
    {
        int dungeonWidth = distanceArray.GetLength(0);
        int dungeonHeight = distanceArray.GetLength(1);

        int visitableRoomCount = 0;

        for(int x = 0; x < dungeonWidth;x++)
        {
            for (int y = 0; y < dungeonHeight;y++)
            {
                if (distanceArray[x, y] >= 0)
                {
                    visitableRoomCount++;
                }
            }
        }

        return visitableRoomCount;
    }

    private void SetRoomTypes(DungeonLevelSO dungeonLevel, RoomInfo[,] roomInfos, int[,] distanceArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        int totalRoomCount = dungeonWidth * dungeonHeight;
        int untypedRoomCount = totalRoomCount - 2;         // Subtract Entrance and Boss

        Dictionary<RoomType, int> roomTypeCountDictionary = new()
        {
            { RoomType.Small, GetRoomCountByPercentage(dungeonLevel.smallRoomPercentage, totalRoomCount) },
            { RoomType.Medium, GetRoomCountByPercentage(dungeonLevel.mediumRoomPercentage, totalRoomCount) },
            { RoomType.Large, GetRoomCountByPercentage(dungeonLevel.largeRoomPercentage, totalRoomCount) }
        };

        int loopCount = 0;
        bool isDone = false;

        while (loopCount < 10000)
        {
            int regularRoomCount = 0;
            foreach (KeyValuePair<RoomType, int> keyValuePair in roomTypeCountDictionary)
            {
                regularRoomCount += keyValuePair.Value;
            }
            if (regularRoomCount >= untypedRoomCount)
            {
                isDone = true;
                break;
            }

            RoomType randomRoomType = GetRandomRegularRoomType();

            roomTypeCountDictionary[randomRoomType]++;
            loopCount++;
        }

        if (!isDone)
        {
            Debug.Log("Unable to Set Regular Room Types");
        }

        /* DEBUG
        foreach (KeyValuePair<RoomType, int> keyValuePair in roomTypeCountDictionary)
        {
            Debug.Log(keyValuePair.Key + " Count : " + keyValuePair.Value);
        }
        */

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (roomInfos[x, y].roomType == RoomType.Entrance || roomInfos[x, y].roomType == RoomType.Boss) 
                {
                    continue;
                }

                loopCount = 0;
                isDone = false;

                while (loopCount < 10000)
                {
                    RoomType selectedRoomType = GetRandomRegularRoomType();

                    if (roomTypeCountDictionary[selectedRoomType] > 0)
                    {
                        roomInfos[x, y].roomType = selectedRoomType;
                        roomTypeCountDictionary[selectedRoomType]--;
                        isDone = true;
                        break;
                    }

                    loopCount++;
                }

                if (!isDone) Debug.Log("Unable to select Room type");
            }
        }

        SetSpecialRoomTypes(dungeonLevel, roomInfos, distanceArray);
    }

    private void SetSpecialRoomTypes(DungeonLevelSO dungeonLevel, RoomInfo[,] roomInfos, int[,] distanceArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        Dictionary<RoomType, int> roomTypeCountDictionary = new()
        {
            { RoomType.Chest, dungeonLevel.chestRoomCount },
            { RoomType.Shop, dungeonLevel.shopRoomCount },
        };

        int specialRoomCount = 0;
        foreach (KeyValuePair<RoomType, int> keyValuePair in roomTypeCountDictionary)
        {
            specialRoomCount += keyValuePair.Value;
        }

        int maxDistance = 0;
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (distanceArray[x, y] > maxDistance)
                {
                    maxDistance = distanceArray[x, y];
                }
            }
        }

        int distanceBetweenSpecialRoom = maxDistance / (specialRoomCount + 1);

        //Debug.Log("Max Distance : " + maxDistance + " And Gap : " + distanceBetweenSpecialRoom);
        //Debug.Log("Boss Room Distance : " + distanceArray[dungeonWidth - 1, 0]);

        for (int count = 1; count <= specialRoomCount; count++)
        {
            int currentDistance = count * distanceBetweenSpecialRoom;
            Vector2Int selectedCoordinate = GetRoomCoordinateByDistance(distanceArray, currentDistance);

            int loopCount = 0;
            bool isDone = false;

            while (loopCount < 10000)
            {
                RoomType selectedRoomType = GetRandomSpecialRoomType();

                if (roomTypeCountDictionary[selectedRoomType] > 0)
                {
                    roomInfos[selectedCoordinate.x, selectedCoordinate.y].roomType = selectedRoomType;
                    roomTypeCountDictionary[selectedRoomType]--;
                    isDone = true;
                    break;
                }

                loopCount++;
            }

            if (!isDone) Debug.Log("Unable to select Room type");
        }

        // NEED TO SET HIDDEN ROOM
    }

    private Vector2Int GetRoomCoordinateByDistance(int[,] distanceArray, int distance)
    {
        int arrayWidth = distanceArray.GetLength(0);
        int arrayHeight = distanceArray.GetLength(1);

        List<Vector2Int> selectedRoomCoordinateList = new();

        for (int x = 0; x < arrayWidth; x++)
        {
            for (int y = 0; y < arrayHeight; y++)
            {
                if (distanceArray[x, y] == distance)
                {
                    selectedRoomCoordinateList.Add(new Vector2Int(x, y));
                }
            }
        }

        if (selectedRoomCoordinateList.Count == 0)
        {
            Debug.Log("There isn't " + distance + " Distance Room in Distance Array");
            return Vector2Int.zero;
        }

        Vector2Int randomlySelectedRoomCoordinate = selectedRoomCoordinateList[Random.Range(0, selectedRoomCoordinateList.Count)];

        #region EXCEPTION CASE
        if (randomlySelectedRoomCoordinate == new Vector2Int(0, arrayHeight - 1))       // If Select Entrance
        {
            List<Vector2Int> adjacentCoordinatList = new();

            int currentCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y];

            int rightCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x + 1, randomlySelectedRoomCoordinate.y];

            if (Mathf.Abs(rightCoordinateDistance - currentCoordinateDistance) == 1)      // If distance == 1
            {
                adjacentCoordinatList.Add(new Vector2Int(randomlySelectedRoomCoordinate.x + 1, randomlySelectedRoomCoordinate.y));
            }

            int downCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y - 1];

            if (Mathf.Abs(downCoordinateDistance - currentCoordinateDistance) == 1)
            {
                adjacentCoordinatList.Add(new Vector2Int(randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y - 1));
            }

            randomlySelectedRoomCoordinate = adjacentCoordinatList[Random.Range(0, adjacentCoordinatList.Count)];
        }

        if (randomlySelectedRoomCoordinate == new Vector2Int(arrayWidth - 1, 0))        // If Select Boss
        {
            List<Vector2Int> adjacentCoordinatList = new();

            int currentCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y];

            int leftCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x - 1, randomlySelectedRoomCoordinate.y];

            if (Mathf.Abs(leftCoordinateDistance - currentCoordinateDistance) == 1)
            {
                adjacentCoordinatList.Add(new Vector2Int(randomlySelectedRoomCoordinate.x - 1, randomlySelectedRoomCoordinate.y));
            }

            int upCoordinateDistance = distanceArray[randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y + 1];

            if (Mathf.Abs(upCoordinateDistance - currentCoordinateDistance) == 1)
            {
                adjacentCoordinatList.Add(new Vector2Int(randomlySelectedRoomCoordinate.x, randomlySelectedRoomCoordinate.y + 1));
            }

            randomlySelectedRoomCoordinate = adjacentCoordinatList[Random.Range(0, adjacentCoordinatList.Count)];

            Debug.Log("Selected Room Coordinate is Boss So " + randomlySelectedRoomCoordinate + " is Selected");
        }
        #endregion EXCEPTION CASE

        return randomlySelectedRoomCoordinate;
    }

    private void SetRoomDetails(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                List<RoomDetailsSO> selectedRoomDetailsList = GetSpecificRoomDetails(selectedDungeonLevel.roomDetailsArray, roomInfos[x, y].roomType);
                roomInfos[x, y].roomDetails = selectedRoomDetailsList[Random.Range(0, selectedRoomDetailsList.Count)];
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

    private RoomType GetRandomSpecialRoomType()
    {
        int specialRoomCount = Random.Range(0, 2);
        if (specialRoomCount == 0)
            return RoomType.Chest;
        else if (specialRoomCount == 1)
            return RoomType.Shop;
        else
            return RoomType.None;
    }

    private void SetHiddenRoom(RoomInfo[,] roomInfos, int[,] distanceArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        int maxDistance = 0;
        Vector2Int farthestPosition = Vector2Int.zero;

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (distanceArray[x, y] > maxDistance)
                {
                    maxDistance = distanceArray[x, y];
                    farthestPosition = new Vector2Int(x, y);
                }
            }
        }

        roomInfos[farthestPosition.x, farthestPosition.y].roomType = RoomType.Hidden;
    }

    private void InstantiateRooms(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        Transform dungeonRooms = new GameObject("Dungeon Rooms").transform;

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                GameObject roomGameObject = Instantiate(roomInfos[x, y].roomDetails.roomPrefab, dungeonRooms);

                Vector3 roomPosition = new Vector3(x * selectedDungeonLevel.roomGap, y * selectedDungeonLevel.roomGap, 0f);
                roomGameObject.transform.localPosition = roomPosition;
                roomInfos[x, y].roomTransform = roomGameObject.transform;

                //roomGameObject.SetActive(false);
            }
        }
    }

    private int[,] BFS(RoomInfo[,] roomInfos, Vector2Int startPosition, List<Vector2Int> directions)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        Queue<Vector2Int> queue = new();
        int[,] visited = new int[dungeonWidth, dungeonHeight];

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                visited[x, y] = -1;
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

        return visited;
    }

    private void InstantiateDoors(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (roomInfos[x, y].isUpConnected)
                {
                    InstantiateDoor(selectedDungeonLevel, roomInfos, x, y, Orientation.Up);
                }
                if (roomInfos[x, y].isDownConnected)
                {
                    InstantiateDoor(selectedDungeonLevel, roomInfos, x, y, Orientation.Down);
                }
                if (roomInfos[x, y].isLeftConnected)
                {
                    InstantiateDoor(selectedDungeonLevel, roomInfos, x, y, Orientation.Left);
                }
                if (roomInfos[x, y].isRightConnected)
                {
                    InstantiateDoor(selectedDungeonLevel, roomInfos, x, y, Orientation.Right);
                }
            }
        }
    }

    private void InstantiateDoor(DungeonLevelSO selectedDungeonLevel, RoomInfo[,] roomInfos, int x, int y, Orientation doorOrientation)
    {
        int connectedRoomOffsetX = 0, connectedRoomOffsetY = 0;
        Orientation oppositeOrientation;
        Vector2 doorOffset;
        if (doorOrientation == Orientation.Up)
        {
            connectedRoomOffsetX = 0;
            connectedRoomOffsetY = 1;

            oppositeOrientation = Orientation.Down;
            doorOffset = new Vector2(0, Settings.doorVerticalOffset);
        }
        else if (doorOrientation == Orientation.Down)
        {
            connectedRoomOffsetX = 0;
            connectedRoomOffsetY = -1;

            oppositeOrientation = Orientation.Up;
            doorOffset = new Vector2(0, -Settings.doorVerticalOffset);
        }
        else if (doorOrientation == Orientation.Left)
        {
            connectedRoomOffsetX = -1;
            connectedRoomOffsetY = 0;

            oppositeOrientation = Orientation.Right;
            doorOffset = new Vector2(-Settings.doorHorizontalOffset, 0);
        }
        else        //Orientation.Right
        {
            connectedRoomOffsetX = 1;
            connectedRoomOffsetY = 0;

            oppositeOrientation = Orientation.Left;
            doorOffset = new Vector2(Settings.doorHorizontalOffset, 0);
        }

        GameObject instantiatedDoor;
        if (doorOrientation == Orientation.Up || doorOrientation == Orientation.Down)
        {
            instantiatedDoor = Instantiate(selectedDungeonLevel.horizontalDoorPrefab, roomInfos[x, y].roomTransform);
        }
        else
        {
            instantiatedDoor = Instantiate(selectedDungeonLevel.verticalDoorPrefab, roomInfos[x, y].roomTransform);
        }

        Doorway doorway = GetSpecificDoorway(roomInfos[x, y].roomDetails.doorwayArray, doorOrientation);

        instantiatedDoor.transform.localPosition = doorway.position;

        if (doorOrientation == Orientation.Left)
        {
            instantiatedDoor.transform.localScale = new Vector3(-Mathf.Abs(instantiatedDoor.transform.localScale.x), instantiatedDoor.transform.localScale.y, 1f);
        }
        if (doorOrientation == Orientation.Up)
        {
            instantiatedDoor.transform.localScale = new Vector3(instantiatedDoor.transform.localScale.x, -Mathf.Abs(instantiatedDoor.transform.localScale.y), 1f);
        }

        Door door = instantiatedDoor.GetComponent<Door>();
        door.connectedPosition = (Vector2)roomInfos[x + connectedRoomOffsetX, y + connectedRoomOffsetY].roomTransform.position + 
            GetSpecificDoorway(roomInfos[x + connectedRoomOffsetX, y + connectedRoomOffsetY].roomDetails.doorwayArray, oppositeOrientation).position +
            doorOffset;
        door.connectedRoomTransform = roomInfos[x + connectedRoomOffsetX, y + connectedRoomOffsetY].roomTransform;
        door.connectedRoomType = roomInfos[x + connectedRoomOffsetX, y + connectedRoomOffsetY].roomType;

        if (roomInfos[x, y].roomType == RoomType.Boss)
        {
            bossRoomDoorList.Add(door);
        }
    }

    private void BlockUnusedDoors(RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                Tilemap[] tilemapArray = roomInfos[x, y].roomTransform.GetComponentsInChildren<Tilemap>();

                if (!roomInfos[x, y].isUpConnected)
                {
                    Doorway upDoorway = GetSpecificDoorway(roomInfos[x, y].roomDetails.doorwayArray, Orientation.Up);

                    BlockVerticalDoor(tilemapArray, upDoorway.doorwayCopyPosition, upDoorway.doorwayCopyWidth, upDoorway.doorwayCopyHeight);
                }
                if (!roomInfos[x, y].isDownConnected)
                {
                    Doorway downDoorway = GetSpecificDoorway(roomInfos[x, y].roomDetails.doorwayArray, Orientation.Down);

                    BlockVerticalDoor(tilemapArray, downDoorway.doorwayCopyPosition, downDoorway.doorwayCopyWidth, downDoorway.doorwayCopyHeight);
                }
                if (!roomInfos[x, y].isLeftConnected)
                {
                    Doorway downDoorway = GetSpecificDoorway(roomInfos[x, y].roomDetails.doorwayArray, Orientation.Left);

                    BlockHorizontalDoor(tilemapArray, downDoorway.doorwayCopyPosition, downDoorway.doorwayCopyWidth, downDoorway.doorwayCopyHeight);
                }
                if (!roomInfos[x, y].isRightConnected)
                {
                    Doorway downDoorway = GetSpecificDoorway(roomInfos[x, y].roomDetails.doorwayArray, Orientation.Right);

                    BlockHorizontalDoor(tilemapArray, downDoorway.doorwayCopyPosition, downDoorway.doorwayCopyWidth, downDoorway.doorwayCopyHeight);
                }
            }
        }
    }

    private void BlockVerticalDoor(Tilemap[] tilemapArray, Vector2Int copyPosition, int copyWidth, int copyHeight)
    {
        foreach (Tilemap tilemap in tilemapArray)
        {
            for (int y = copyPosition.y; y < copyPosition.y + copyHeight; y++)
            {
                TileBase copiedTile = tilemap.GetTile(new Vector3Int(copyPosition.x, y, 0));

                for (int x = copyPosition.x + 1; x < copyPosition.x + copyWidth; x++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), copiedTile);

                    tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), Matrix4x4.identity);
                }
            }
        }
    }

    private void BlockHorizontalDoor(Tilemap[] tilemapArray, Vector2Int copyPosition, int copyWidth, int copyHeight)
    {
        foreach (Tilemap tilemap in tilemapArray)
        {
            for (int x = copyPosition.x; x < copyPosition.x + copyWidth; x++)
            {
                TileBase copiedTile = tilemap.GetTile(new Vector3Int(x, copyPosition.y, 0));

                for (int y = copyPosition.y + 1; y < copyPosition.y + copyHeight; y++)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), copiedTile);
                }
            }
        }
    }

    public void BlockBossRoomDoor()
    {
        foreach (Door door in bossRoomDoorList)
        {
            door.gameObject.SetActive(false);
        }

        if (bossRoomInfo.isUpConnected)
        {
            InstantiatePlayerBlocking(Orientation.Up);
        }
        if (bossRoomInfo.isDownConnected)
        {
            InstantiatePlayerBlocking(Orientation.Down);
        }
        if (bossRoomInfo.isLeftConnected)
        {
            InstantiatePlayerBlocking(Orientation.Left);
        }
        if (bossRoomInfo.isRightConnected)
        {
            InstantiatePlayerBlocking(Orientation.Right);
        }
    }

    private void InstantiatePlayerBlocking(Orientation doorOrientation)
    {
        Doorway doorway = GetSpecificDoorway(bossRoomInfo.roomDetails.doorwayArray, doorOrientation);
        Vector2 positionOffset;
        GameObject instantiatedBlocking;
        if (doorOrientation == Orientation.Up)
        {
            positionOffset = new Vector2(0f, -0.5f);
            instantiatedBlocking = Instantiate(verticalPlayerBlockingPrefab, bossRoomInfo.roomTransform);
        }
        else if (doorOrientation == Orientation.Down)
        {
            positionOffset = new Vector2(0f, 0.5f);
            instantiatedBlocking = Instantiate(verticalPlayerBlockingPrefab, bossRoomInfo.roomTransform);
        }
        else if (doorOrientation == Orientation.Left)
        {
            positionOffset = new Vector2(0.5f, 0f);
            instantiatedBlocking = Instantiate(horizontalPlayerBlockingPrefab, bossRoomInfo.roomTransform);
        }
        else
        {
            positionOffset = new Vector2(-0.5f, 0f);
            instantiatedBlocking = Instantiate(horizontalPlayerBlockingPrefab, bossRoomInfo.roomTransform);
        }

        instantiatedBlocking.transform.localPosition = doorway.position + positionOffset;

        instantiatedBlockingList.Add(instantiatedBlocking);
    }

    public void UnblockBossRoomDoor()
    {
        foreach (Door door in bossRoomDoorList)
        {
            door.gameObject.SetActive(true);
        }

        foreach (GameObject instantiatedBlocking in instantiatedBlockingList)
        {
            instantiatedBlocking.SetActive(false);
        }
    }

    private void InstantiateShops(RoomInfo[,] roomInfos)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0;  y < dungeonHeight; y++)
            {
                if (roomInfos[x, y].roomType == RoomType.Shop)
                {
                    Vector2[] spawnPositionArray = roomInfos[x, y].roomDetails.spawnPositionArray;

                    if (spawnPositionArray.Length == 0) continue;

                    Vector2 spawnPosition = spawnPositionArray[Random.Range(0, spawnPositionArray.Length)];

                    GameObject instantiatedShop = Instantiate(shopPrefab, roomInfos[x, y].roomTransform);

                    instantiatedShop.transform.localPosition = spawnPosition;
                }
            }
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

    private Doorway GetSpecificDoorway(Doorway[] doorwayArray, Orientation orientation)
    {
        foreach (Doorway doorway in doorwayArray)
        {
            if (doorway.orientation == orientation)
            {
                return doorway;
            }
        }

        Debug.Log("Unable to find " + orientation + " doorway in " + doorwayArray);
        return null;
    }

    #region DEBUG
    private void PrintRoomInfo(RoomInfo[,] roomInfos, int[,] distanceArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        for (int x = 0 ; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                int realPosX = x * 2 + 1;
                int realPosY = y * 2 + 1;

                if (!roomInfos[x, y].isDownConnected)
                {
                    Instantiate(block, new Vector3(realPosX, realPosY - 1, 0), Quaternion.identity);
                }
                if (!roomInfos[x, y].isUpConnected)
                {
                    Instantiate(block, new Vector3(realPosX, realPosY + 1, 0), Quaternion.identity);
                }
                if (!roomInfos[x, y].isLeftConnected)
                {
                    Instantiate(block, new Vector3(realPosX - 1, realPosY, 0), Quaternion.identity);
                }
                if (!roomInfos[x, y].isRightConnected)
                {
                    Instantiate(block, new Vector3(realPosX + 1, realPosY, 0), Quaternion.identity);
                }
                Instantiate(block, new Vector3(realPosX - 1, realPosY - 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX - 1, realPosY + 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX + 1, realPosY - 1, 0), Quaternion.identity);
                Instantiate(block, new Vector3(realPosX + 1, realPosY + 1, 0), Quaternion.identity);
            
                if (distanceArray[x, y] >= 0)
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
