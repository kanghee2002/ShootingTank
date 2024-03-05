using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public float minJumpVelocity;
    public float fallVelocity;

    public PlatformCheckObject platformCheckObject;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        minJumpVelocity = jumpPower - minJumpVelocity;
        fallVelocity = jumpPower - fallVelocity;
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(moveInput * moveSpeed, rigid.velocity.y);
        if (moveInput < 0) spriteRenderer.flipX = false;
        else if (moveInput > 0) spriteRenderer.flipX = true;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && platformCheckObject.isTouchingPlatform == true)
        {
            platformCheckObject.isTouchingPlatform = false;
            rigid.velocity = Vector2.zero;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
        }
        
        if (rigid.velocity.y > fallVelocity && rigid.velocity.y < minJumpVelocity)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                rigid.velocity = new Vector2(rigid.velocity.x, fallVelocity);
            }
        }
    }
}
