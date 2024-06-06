using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpPack : MonoBehaviour
{
    private Rigidbody2D rigid;
    private PlayerController playerController;

    [SerializeField]
    private float fallSpeed;

    [SerializeField]
    private float maxGauge;
    private float curGauge;

    [SerializeField]
    private float useSpeed;

    [SerializeField]
    private float fillInterval;

    [SerializeField]
    private float fillSpeed;

    private IEnumerator curFillCoroutine;



    //Re
    private Slider slider;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody2D>();
        curGauge = maxGauge;


        //Re
        slider = GameObject.Find("JumpPackSlider").GetComponent<Slider>();
    }

    private void Update()
    {
        //Re
        slider.value = curGauge / maxGauge;




        if (rigid.velocity.y < 0 && Input.GetKey(KeyCode.Space) && curGauge > 0f)
        {
            if (curFillCoroutine != null)
            {
                StopCoroutine(curFillCoroutine);
                curFillCoroutine = null;
            }
            Use();
        }
        else
        {
            if (curFillCoroutine == null)
            {
                curFillCoroutine = FillGauge();
                StartCoroutine(curFillCoroutine);
            }
        }
    }

    private void Use()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, fallSpeed);
        curGauge -= useSpeed;
    }

    private IEnumerator FillGauge()
    {
        yield return new WaitForSeconds(fillInterval);

        while (curGauge < maxGauge)
        {
            curGauge += fillSpeed;
            yield return new WaitForFixedUpdate();
        }
    }
}
