﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;


public class PlayerController : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpPower;

    [SerializeField] private int maxJumpCount;

    [SerializeField] private float downFallPower;

    [SerializeField] private float downFallCoolTime;

    [Header("Additional Settings")]
    [SerializeField] private List<Transform> weaponParents;

    [SerializeField] private JumpChecker jumpChecker;

    public Transform utilityAnchorTransform;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    private const string playerLayerName = "Player";
    private const string oneWayPlatformLayerName = "OneWayPlatform";

    private int jumpCount;
    private JumpState jumpState;

    private bool isStunning = false;

    private float downFallTimer;

    private bool canJumpInfinitely;

    public int MaxJumpCount { get => maxJumpCount; }

    public JumpState GetJumpState() => jumpState;

    public void AllowInfiniteJump(bool canJumpInfinitely) => this.canJumpInfinitely = canJumpInfinitely;

    private int maxMoveSpeed = 20;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        isStunning = false;
        jumpCount = maxJumpCount;
        jumpState = JumpState.NotJumping;
        downFallTimer = 0f;
        canJumpInfinitely = false;
    }

    private void Update()
    {
        SetJumpVariables();
        CoolDownDownFallTIme();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isStunning)
        {
            return;
        }

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

            transform.localScale = new Vector3(xDir * Mathf.Abs(transform.localScale.x),
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (jumpState == JumpState.Jumping || jumpState == JumpState.Falling)
            {
                if (downFallTimer <= 0f)
                {
                    DownFall();
                }
            }
        }
    }

    private void Jump()
    {
        if (isStunning)
        {
            return;
        }

        rigid.velocity = Vector2.zero;
        rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
    }

    private void DownJump()
    {
        if (isStunning)
        {
            return;
        }

        StartCoroutine(IgnoreCollisionRoutine(polygonCollider, jumpChecker.oneWayPlatformCollider));
    }

    private void DownFall()
    {
        rigid.AddForce(Vector2.down * downFallPower * rigid.mass);

        downFallTimer = downFallCoolTime;
    }

    private void CoolDownDownFallTIme()
    {
        if (downFallTimer > 0f)
        {
            downFallTimer -= Time.deltaTime;
        }
    }

    private IEnumerator IgnoreCollisionRoutine(Collider2D collider1, Collider2D collider2)
    {
        Physics2D.IgnoreCollision(collider1, collider2, true);

        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreCollision(collider1, collider2, false);
    }

    public void KnockBack(Vector2 force, float time)
    {
        isStunning = true;
        rigid.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(StopStunRoutine(time));
    }

    private IEnumerator StopStunRoutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        isStunning = false;
    }

    private void MinusJumpCount()
    {
        if (canJumpInfinitely) return;

        if (jumpCount > 0) jumpCount--;
    }

    public void AddMaxJumpCount(int count) => maxJumpCount += count;

    public void AddJumpPowerValue(float power) => jumpPower += power;

    public void AddMoveSpeedValue(float speed)
    {
        moveSpeed += speed;

        if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
    }

    public void MinusDownFallCoolTime(float time)
    {
        if (downFallCoolTime - time <= 0.2f)
        {
            downFallCoolTime = 0.2f;
            return;
        }
        downFallCoolTime -= time;
    }
}
