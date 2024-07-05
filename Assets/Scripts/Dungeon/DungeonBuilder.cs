using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBuilder : Singleton<DungeonBuilder>
{
    [SerializeField] private DungeonLevelSO[] dungeonLevelSOArray;

    // BFS 알고리즘 이용해서 Entrance와 제일 먼 곳에 HiddenRoom 배치
    // 아래 순회 알고리즘 이용해서 ?? 퍼센트도 못다니면 다시 만들기

    /*private DungeonLevelSO selectedDungeonLevel;
    private int dungeonWidth;
    private int dungeonHeight;
    private RoomInfo[,] roomInfos;*/

    //DEBUG
    [SerializeField] private GameObject block;


    private struct RoomInfo
    {
        public RoomDetailsSO roomDetails;
        public bool isUpConnected;
        public bool isDownConnected;
        public bool isLeftConnected;
        public bool isRightConnected;
    }

    private void Start()
    {
        BuildDungeon(1);
    }

    public void BuildDungeon(int level)
    {
        DungeonLevelSO selectedDungeonLevel = SelectDungeonLevelSO(level);

        int dungeonWidth = selectedDungeonLevel.dungeonWidth;
        int dungeonHeight = selectedDungeonLevel.dungeonHeight;

        RoomInfo[ , ] roomInfos = new RoomInfo[dungeonWidth, dungeonHeight];

        InitializeDungeon(roomInfos);
        

        SetDefaultRoomDetails(roomInfos, selectedDungeonLevel.roomDetails);

        SetDoors(roomInfos, selectedDungeonLevel.horizontallyBlockedDoorCountPerFloor, selectedDungeonLevel.verticallyConnectedDoorCountPerFloor);



        PrintRoomInfo(roomInfos);
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
            }
        }
    }

    private void SetDefaultRoomDetails(RoomInfo[,] roomInfos, RoomDetailsSO[] roomDetailsArray)
    {
        int dungeonWidth = roomInfos.GetLength(0);
        int dungeonHeight = roomInfos.GetLength(1);

        // Set Entrance Room
        List<RoomDetailsSO> entranceRoomDetailsList = GetSpecificRoom(roomDetailsArray, RoomType.Entrance);
        roomInfos[0, dungeonHeight - 1].roomDetails = entranceRoomDetailsList[Random.Range(0, entranceRoomDetailsList.Count)];

        // Set Boss Room
        List<RoomDetailsSO> bossRoomDetailsList = GetSpecificRoom(roomDetailsArray, RoomType.Boss);
        roomInfos[dungeonWidth - 1, 0].roomDetails = bossRoomDetailsList[Random.Range(0, bossRoomDetailsList.Count)];
    }

    private void SetDoors(RoomInfo[,] roomInfos, int horizontallyBlockedDoorCountPerFloor, int verticallyConnectedDoorCountPerFloor)
    {
        int dungeonWidth = roomInfos.GetLength(0);
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

    private List<RoomDetailsSO> GetSpecificRoom(RoomDetailsSO[] roomDetailsArray, RoomType roomType)
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


    #region DEBUG
    private void PrintRoomInfo(RoomInfo[,] roomInfos)
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
            }
        }
    }

    #endregion DEBUG
}
