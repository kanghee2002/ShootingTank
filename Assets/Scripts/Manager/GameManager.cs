using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameObject playerPrefab;

    public GameObject playerObj { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InstantiatePlayer();

        #region TEST_SCENE
        if (SceneManager.GetActiveScene().name == "TEST")
        {
            MakePlayerActive();
        }
        #endregion
    }

    private void InstantiatePlayer()
    {
        //Load Data Files and ...
        playerObj = Instantiate(playerPrefab, transform);
        playerObj.SetActive(false);
    }

    private void Update()
    {
        //[DEBUG]
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MakePlayerActive();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            WeaponManager.Instance.IsRightWeaponEnabled = !WeaponManager.Instance.IsRightWeaponEnabled;
        }
    }

    public void MakePlayerActive()
    {
        playerObj.transform.parent = null;
        playerObj.SetActive(true);
        playerObj.transform.position = Vector3.zero;
    }

    public void MakePlayerInactive()
    {
        playerObj.transform.parent = transform;
        playerObj.SetActive(false);
    }
}
