using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public float moveSpeed;
    public float jumpPower;
    public float minJumpVelocity;
    public float fallVelocity;

    public PlatformCheckObject platformCheckObject;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private IDamageable damageableInstance;

    [SerializeField]
    private float hp;
    float IDamageable.Hp { get => hp; set => hp = value; }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageableInstance = this;
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
        if (moveInput != 0)
        {
            Vector3 scaleVec;
            if (moveInput < 0)
            {
                scaleVec = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                scaleVec = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }

            transform.localScale = scaleVec;
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.localScale = scaleVec;
            }
        }
        
        
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

    void IDamageable.Damage(float damageAmount)
    {
        damageableInstance.Hp -= damageAmount;

        if (damageableInstance.Hp <= 0)
        {
            damageableInstance.Die();
        }
    }

    void IDamageable.Die()
    {
        //Do Die
    }
}
