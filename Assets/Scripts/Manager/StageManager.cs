using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    protected override void Awake()
    {
        //Destroy on Load
        GameManager.Instance.MakePlayerActive();
    }

    public void FinishStage()
    {
        //Send Data to GameManager
        //Load Scene -> Use coroutine to invoke scene loading
        GameManager.Instance.MakePlayerInactive();
        SceneManager.LoadScene("Shop");
    }
}
