using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private CameraController cameraController;

    [HideInInspector] public Transform lastRoomTransform;
    [HideInInspector] public Transform currentRoomTransform;

    protected override void Awake()
    {
        GameManager.Instance.ChangeGameState(GameState.playingLevel);

        lastRoomTransform = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(Settings.stageSelectScene);
            GameManager.Instance.ChangeGameState(GameState.gameStarted);
        }
    }

    public void OnEnterRoom(Transform roomTransform, RoomType roomType)
    {
        lastRoomTransform = currentRoomTransform;
        currentRoomTransform = roomTransform;

        cameraController.transform.position = GameManager.Instance.playerObject.transform.position;

        if (roomType != RoomType.Boss)
        {
            GameManager.Instance.ChangeGameState(GameState.enterRoom);
        }
        else        //RoomType.Boss
        {
            GameManager.Instance.ChangeGameState(GameState.enterBossRoom);
        }
    }

    public void FinishStage()
    {
        //Send Data to GameManager
        //Load Scene -> Use coroutine to invoke scene loading
        GameManager.Instance.MakePlayerInactive();
        SceneManager.LoadScene(Settings.stageSelectScene);
    }
}
