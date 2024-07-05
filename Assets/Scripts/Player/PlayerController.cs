using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpPower;

    [SerializeField] private int maxJumpCount;

    [Header("Additional Settings")]
    [SerializeField] private List<Transform> weaponParents;

    [SerializeField] private PlayerJumpChecker playerJumpChecker;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private int jumpCount;
    private JumpState jumpState;
    public JumpState GetJumpState() => jumpState;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        jumpCount = maxJumpCount;
        jumpState = JumpState.NotJumping;
    }

    private void Update()
    {
        SetJumpVariables();

        //Debug.Log("JumpCount = " + jumpCount + " | JumpState = " + jumpState.ToString());
        //Debug.Log(maxJumpCount);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(moveInput * moveSpeed, rigid.velocity.y);
        if (moveInput != 0)
        {
            int xDir;
            if (moveInput < 0)
            {
                xDir = 1;
            }
            else
            {
                xDir = -1;
            }

            transform.localScale = new Vector3(xDir,
                transform.localScale.y, transform.localScale.z);
            foreach(var weaponObj in weaponParents)
            {
                weaponObj.localScale = new Vector3(xDir, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void SetJumpVariables()
    {
        if (jumpState == JumpState.NotJumping && !playerJumpChecker.isGrounding)
        {
            MinusJumpCount();

            if (jumpCount == 0)
            {
                jumpState = JumpState.Falling;
            }
            else
            {
                jumpState = JumpState.Jumping;
            }
        }

        if ((jumpState == JumpState.Falling || jumpState == JumpState.Jumping)
            && playerJumpChecker.isGrounding)
        {
            jumpCount = maxJumpCount;
            jumpState = JumpState.NotJumping;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (jumpState)
            {
                case JumpState.NotJumping:
                case JumpState.Jumping:

                    Jump();

                    MinusJumpCount();

                    if (jumpCount > 0)
                    {
                        jumpState = JumpState.Jumping;
                    }
                    else
                    {
                        jumpState = JumpState.Falling;
                    }
                    break;

                case JumpState.Falling:
                default:
                    break;
            }
        }
    }

    private void Jump()
    {

        rigid.velocity = Vector2.zero;
        rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
    }

    private void MinusJumpCount()
    {
        if (jumpCount > 0) jumpCount--;
    }

    public void AddMaxJumpCount(int count) => maxJumpCount += count;
    public void MinusMaxJumpCount(int count) => maxJumpCount -= count;

    public void AddJumpPowerValue(float power) => jumpPower += power;
    public void MinusJumpPowerValue(float power) => jumpPower -= power;

    public void AddMoveSpeedValue(float speed) => moveSpeed += speed;
    public void MinusMoveSpeedValue(float speed) => moveSpeed -= speed;
}
