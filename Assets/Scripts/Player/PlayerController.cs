using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Default Settings")]
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get; set; }

    [SerializeField]
    private float jumpPower;
    public float JumpPower { get; set; }

    [SerializeField]
    private float minJumpVelocity;
    public float MinJumpVelocity { get; set; }

    [SerializeField]
    private float fallVelocity;
    public float FallVelocity { get; set; }

    [SerializeField]
    private float maxHp;

    [SerializeField]
    private float curHp;

    [Header("Additional Settings")]
    [SerializeField]
    private List<Transform> weaponParents;

    [SerializeField]
    private PlayerJumpChecker playerJumpChecker;

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

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerJumpChecker.canJump == true)
        {
            playerJumpChecker.canJump = false;
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
        curHp -= damageAmount;

        if (curHp <= 0)
        {
            ((IDamageable)this).Die();
        }
    }

    void IDamageable.Die()
    {
        //Do Die
        Debug.Log("Player Die");
    }
}
