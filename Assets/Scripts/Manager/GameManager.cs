using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameObject playerPrefab;

    public GameObject playerObj { get; private set; }

    private void Awake()
    {
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        //Load Data Files
        playerObj = Instantiate(playerPrefab, transform);
        playerObj.SetActive(false);
    }

    private void Update()
    {
        //[DEBUG]
        MakePlayerActive();
    }

    //[DEBUG]
    private void MakePlayerActive()
    {
        if (Input.GetKeyDown(KeyCode.Z)) {
            playerObj.transform.parent = null;
            playerObj.SetActive(true);
            playerObj.transform.position = Vector3.zero;
        }
    }
}
