using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectUIController : MonoBehaviour
{
    [SerializeField] private Button gamePlayBtn;

    private void Start()
    {
        gamePlayBtn.onClick.AddListener(() => SceneManager.LoadScene("Stage1"));
    }
}
