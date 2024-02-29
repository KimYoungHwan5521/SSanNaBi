using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public struct ContactInfo
{
    // 부딪힌 오브젝트
    // 오브젝트 콘택트 정보
    // 부딪힌 시간
    public GameObject other;
    public ContactPoint2D contact;
    public float time;

    public ContactInfo(GameObject other, ContactPoint2D contact, float time)
    {
        this.other = other;
        this.contact = contact;
        this.time = time;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Character : Breakable
{
    public Rigidbody2D rigid;
    public Animator anim;

    public Vector2 preferDirection;
    public Vector2 moveDirection;
    public Vector2 faceDirection;

    public float moveSpeed;
    public float jumpPower;

    bool isGround = false;
    protected List<ContactInfo> collisionList = new List<ContactInfo>();

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        moveDirection = preferDirection * Vector2.right;
        moveDirection.Normalize();
        if(moveDirection.magnitude > 0)
        {
            rigid.AddForce(moveDirection, ForceMode2D.Impulse);
            Vector2 speedLimitVelocity = rigid.velocity;
            speedLimitVelocity.x = Mathf.Clamp(speedLimitVelocity.x, -moveSpeed, moveSpeed);
            rigid.velocity = speedLimitVelocity;
            faceDirection = rigid.velocity * Vector2.right;
            faceDirection.Normalize();
        }
        else
        {
            rigid.velocity *= Vector2.up;
        }
        if(transform.localScale.x * faceDirection.x < 0)
        {
            transform.localScale *= new Vector2(-1, 1);
        }
        anim.SetFloat("speed", rigid.velocity.magnitude);
    }


    protected virtual void OnMove(InputValue value) 
    {
        Move(value.Get<Vector2>());
    }

    protected virtual void Move(Vector2 direction)
    {
        direction.Normalize();
        preferDirection= direction;
    }

    protected virtual void OnJump()
    {
        Jump();
    }

    protected virtual void Jump()
    {
        rigid.AddForce(Vector2.up * jumpPower);
    }

    protected void CheckGround()
    {

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contacts.Length];

        // 콘택트 정보를 받아서 contacts에 넣기
        collision.GetContacts(contacts);

        
    }
}
