using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;

    private Rigidbody2D rigid;

    private void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(moveInput * moveSpeed, rigid.velocity.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.velocity = Vector2.zero;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
        }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
