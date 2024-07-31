using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public GameObject currentRoomObject;

    protected override void Awake()
    {
        GameManager.Instance.MakePlayerActive();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("StageSelect");
            GameManager.Instance.ChangeGameState(GameState.gameStarted);
        }
    }

    public void OnEnterRoom(GameObject roomObject, RoomType roomType)
    {
        if (currentRoomObject != null)
        {
            // Disable Enemies
            EnemySpawner.Instance.SetActiveEnemiesInRoom(currentRoomObject.transform, false, false);
        }
        currentRoomObject = roomObject;

        if (roomType != RoomType.Boss)
        {
            EnemySpawner.Instance.SetActiveEnemiesInRoom(currentRoomObject.transform, true, false);
        }
        else        //RoomType.Boss
        {
            EnemySpawner.Instance.SetActiveEnemiesInRoom(currentRoomObject.transform, true, true);
        }
    }

    public void FinishStage()
    {
        //Send Data to GameManager
        //Load Scene -> Use coroutine to invoke scene loading
        GameManager.Instance.MakePlayerInactive();
        SceneManager.LoadScene("StageSelect");
    }
}
