using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private Button shopBtn;

    private void Start()
    {
        shopBtn.onClick.AddListener(() => SceneManager.LoadScene("StageSelect"));
    }
}
