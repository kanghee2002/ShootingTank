using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PlayerController : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpPower;

    [SerializeField] private int maxJumpCount;

    [Header("Additional Settings")]
    [SerializeField] private List<Transform> weaponParents;

    [SerializeField] private JumpChecker jumpChecker;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    private int jumpCount;
    private JumpState jumpState;
    public JumpState GetJumpState() => jumpState;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        jumpCount = maxJumpCount;
        jumpState = JumpState.NotJumping;
    }

    private void Update()
    {
        SetJumpVariables();
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
        if (jumpState == JumpState.NotJumping && !jumpChecker.isGrounding)
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
            && jumpChecker.isGrounding)
        {
            jumpCount = maxJumpCount;
            jumpState = JumpState.NotJumping;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
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

        if (Input.GetKeyDown(KeyCode.S) && jumpChecker.isGroundingOneWayPlatform)
        {
            DownJump();
        }
    }

    private void Jump()
    {
        rigid.velocity = Vector2.zero;
        rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
    }

    private void DownJump()
    {
        StartCoroutine(IgnoreCollisionRoutine(polygonCollider, jumpChecker.oneWayPlatformCollider));
    }

    private IEnumerator IgnoreCollisionRoutine(Collider2D collider1, Collider2D collider2)
    {
        Physics2D.IgnoreCollision(collider1, collider2, true);

        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreCollision(collider1, collider2, false);
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
