using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameObject playerPrefab;

    public GameObject playerObj { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InstantiatePlayer();
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
