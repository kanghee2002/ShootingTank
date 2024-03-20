using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField]
    private Button GamePlayBtn;

    private void Start()
    {
        GamePlayBtn.onClick.AddListener(() =>
            SceneManager.LoadScene("GamePlay")

        );
    }
}
