using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    public GameObject playerObject { get; private set; }

    private GameState gameState;
    public GameState currentGameState { get => gameState; }

    private int currentDungeonLevel;
    public int CurrentDungeonLevel { get => currentDungeonLevel; set => currentDungeonLevel = value; }

    protected override void Awake()
    {
        base.Awake();

        currentDungeonLevel = 1;
        InstantiatePlayer();

        gameState = GameState.gameStarted;
        ChangeGameState(GameState.gameStarted);


        #region TEST_SCENE
        if (SceneManager.GetActiveScene().name == Settings.testScene)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");

            currentDungeonLevel = 1;
        }
        #endregion
    }

    private void InstantiatePlayer()
    {
        //Load Data Files and ...
        playerObject = Instantiate(playerPrefab, transform);
        playerObject.SetActive(false);
    }

    private void Update()
    {
        #region DEBUG
        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerObject.transform.position = DungeonBuilder.Instance.GetBossRoomInfo().roomTransform.position + new Vector3(5f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            WeaponManager.Instance.IsRightWeaponEnabled = !WeaponManager.Instance.IsRightWeaponEnabled;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DungeonBuilder.Instance.BlockBossRoomDoor();
        }
        #endregion

        ExcuteGameStateAction();
    }

    public void MakePlayerActive()
    {
        playerObject.transform.SetParent(null);
        playerObject.SetActive(true);
    }

    public void MakePlayerInactive()
    {
        playerObject.transform.parent = transform;
        playerObject.SetActive(false);
    }

    public void ChangeGameState(GameState newGameState)
    {
        OnGameStateChanged(gameState, newGameState);
    }

    private void OnGameStateChanged(GameState lastGameState, GameState newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState.gameStarted:
                currentDungeonLevel = 1;
                playerObject.SetActive(false);
                break;
            case GameState.playingLevel:
                if (lastGameState == GameState.gameStarted)
                {
                    MakePlayerActive();
                    ChangeGameState(GameState.loadLevel);
                }
                break;
            case GameState.loadLevel:
                ChangeGameState(GameState.playingLevel);
                break;
            case GameState.enterRoom:
                // Make Last Room Dark
                // Make Only Current Room Bright
                if (StageManager.Instance.lastRoomTransform != null)
                {
                    EnemySpawner.Instance.SetActiveEnemiesInRoom(StageManager.Instance.lastRoomTransform, false, false);
                }
                EnemySpawner.Instance.SetActiveEnemiesInRoom(StageManager.Instance.currentRoomTransform, true, false);

                ChangeGameState(GameState.playingLevel);
                break;
            case GameState.enterBossRoom:
                // Boss Cut Scene
                if (StageManager.Instance.lastRoomTransform != null)
                {
                    EnemySpawner.Instance.SetActiveEnemiesInRoom(StageManager.Instance.lastRoomTransform, false, false);
                }
                EnemySpawner.Instance.SetActiveEnemiesInRoom(StageManager.Instance.currentRoomTransform, true, true);
                DungeonBuilder.Instance.BlockBossRoomDoor();

                ChangeGameState(GameState.bossStage);
                break;
            case GameState.bossStage:
                break;
            case GameState.levelCompleted:
                // Set Flag that can move player to next Stage and plus Dungeon level

                DungeonBuilder.Instance.UnblockBossRoomDoor();

                ChangeGameState(GameState.playingLevel);
                // if level == lastLevel => ChagneState(gameWon)
                break;
            case GameState.gameWon:
                // Set UI
                break;
            case GameState.gameLost:
                // Set UI
                break;
            case GameState.gamePaused:
                // Set UI
                break;
            case GameState.restartGame:
                // Reset Data and Load Level 1
                break;
        }
    }

    private void ExcuteGameStateAction()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                break;
            case GameState.loadLevel:
                break;
            case GameState.playingLevel:
                break;
            case GameState.enterRoom:
                break;
            case GameState.enterBossRoom:
                break;
            case GameState.bossStage:
                // if Boss dies, ChangeState
                break;
            case GameState.levelCompleted:
                break;
            case GameState.gameWon:
                // Button will detect and Change State
                break;
            case GameState.gameLost:
                // Button will detect and Change State
                break;
            case GameState.gamePaused:
                // Button will detect and Change State
                break;
            case GameState.restartGame:
                // Re Instantiate Player
                break;
        }
    }
}
