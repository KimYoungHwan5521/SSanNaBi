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

    public float coyoteTime = 0.02f;

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
        isGround = CheckGround();
        // 이동
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
        anim.SetBool("isGround", isGround);
        anim.SetBool("isFall", rigid.velocity.y < 0);
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
        if (!isGround) return;
        rigid.AddForce(Vector2.up * jumpPower);
    }

    protected virtual void OnAttack()
    {
        Attack();
    }

    protected virtual void Attack()
    {
        anim.SetTrigger("doAttack");
    }

    protected virtual void GenerateHitBox(string hitBoxName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Hitboxes/{hitBoxName}");
        GameObject inst = Instantiate(prefab, transform.position, transform.rotation);
        inst.transform.localScale = new Vector2(transform.localScale.x, inst.transform.localScale.y);
        inst.AddComponent<DamageComponent>().Initialize(this, attackDamage);
    }

    protected bool CheckGround()
    {
        // 코요테타임이 지난 콘택트는 삭제
        collisionList.RemoveAll(target => target.time < Time.time - coyoteTime);
        if (rigid.velocity.y == 0) return true;
        // 각도가 46도 이하면 점프 가능한 땅으로 간주
        return collisionList.FindIndex(target => Vector2.Angle(target.contact.normal, Vector2.up) < 46.0f) >= 0;
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        // 정보를 받을 콘택트 선언
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        // 콜리젼의 콘택트 정보를 받아서 contacts에 넣기
        collision.GetContacts(contacts);
        // 콜리젼의 콘택트 정보들 중 이 오브젝트와 접촉 중인 콘택트 정보 찾기
        ContactPoint2D myContact = System.Array.Find(contacts, target => target.otherCollider.gameObject == gameObject);

        // 이미 콜리젼리스트에 들어있는 것을 더하지 않도록 중복체크
        int collisionIndex = collisionList.FindIndex(target => target.other == collision.gameObject);
        if(collisionIndex == -1)
        {
            // 똑같은 것이 없다면 리스트에 추가
            collisionList.Add(new ContactInfo(collision.gameObject, myContact, Time.time));
        }
        else 
        {
            // 이미 들어 있다면 타임만 갱신
            collisionList[collisionIndex] = new ContactInfo(collision.gameObject, myContact, Time.time);
        }
        
    }
}
