using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDisplay : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private List<Sprite> spriteList;

    private SpriteRenderer spriteRenderer;

    private float timer;
    private int index;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 0;
        index = 0;
    }

    private void Update()
    {
        if (timer > 1f)
        {
            index++;
            if (index >= spriteList.Count) index = 0;
            spriteRenderer.sprite = spriteList[index];
            timer = 0f;
        }
        timer += Time.deltaTime;
    }
}
