using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpPack : PlayerUtility
{
    public enum Category
    {
        Broken,
        Old,        // riseSpeed < 0f
        Nice        // riseSpeed > 0f
    }

    public JumpPack(Category category, float riseSpeed, float maxGauge = 3f, 
        float useSpeed = 0.01f, float fillInterval = 1f, float fillSpeed = 0.05f)
    {
        this.category = category;
        this.riseSpeed = riseSpeed;
        this.maxGauge = maxGauge;
        this.useSpeed = useSpeed;
        this.fillInterval = fillInterval;
        this.fillSpeed = fillSpeed;
    }

    private Rigidbody2D rigid;
    private PlayerController playerController;

    [SerializeField]
    private Category category;

    [SerializeField]
    private float riseSpeed;

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
        playerController = playerTransform.GetComponent<PlayerController>();
        rigid = playerTransform.GetComponent<Rigidbody2D>();
        curGauge = maxGauge;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && curGauge > 0f 
            && playerController.GetJumpState() == JumpState.Falling)
        {
            if (category == Category.Broken && rigid.velocity.y >= 0f)
            {
                return;
            }
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
        rigid.velocity = new Vector2(rigid.velocity.x, riseSpeed);
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
