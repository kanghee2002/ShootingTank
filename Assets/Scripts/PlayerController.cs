using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private Vector2 inputVec;

    private Rigidbody2D rigid;

    private void Move()
    {
        Vector2 moveVelocity = inputVec.normalized * moveSpeed;
        rigid.MovePosition(rigid.position + moveVelocity * Time.fixedDeltaTime);
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        Move();
    }
}
