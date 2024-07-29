using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectUIController : MonoBehaviour
{
    [SerializeField] private Button playLevelButton;

    private void Start()
    {
        playLevelButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Stage");
            GameManager.Instance.ChangeGameState(GameState.playingLevel);
        });
    }
}
