using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Status { Normal, Death }

public struct ContactInfo
{
    // �ε��� ������Ʈ
    // ������Ʈ ����Ʈ ����
    // �ε��� �ð�
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
// [RequireComponent(typeof(Animator))]
public class Character : Breakable
{
    public Rigidbody2D rigid;
    public Animator anim;

    public float coyoteTime = 0.02f;

    public Status status = Status.Normal;

    public Vector2 preferDirection;
    public Vector2 moveDirection;
    public Vector2 faceDirection;

    public override bool IsBreak 
    { 
        get => base.IsBreak; 
        set
        {
            base.IsBreak = value;
            if(value == true)
            {
                status = Status.Death;
                if(anim != null)
                {
                    anim.SetTrigger("doDeath");

                }
            }
        }

    }

    public float moveSpeed;
    public float jumpPower;

    bool isGround = false;
    public bool isHit = false;
    public bool isAttack = false;
    private bool _isGrab = false;
    public bool IsGrab
    {
        get => _isGrab;
        set
        {
            if(value == false)
            {
                beGrabed = null;
            }
            _isGrab = value;
        }
    }
    public GameObject beGrabed;
    protected List<ContactInfo> collisionList = new List<ContactInfo>();

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        curInvincibleTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // �׷� ������ ���� ���� Ȥ�� �����ʿ��� ����� ��
        int beGrabedCollisionIndex = collisionList.FindIndex(target => target.other == beGrabed);
        // �븻�� x�� 0�̸� ���� �Ʒ����� ������.
        bool isGrabSide = beGrabedCollisionIndex != -1 && collisionList[beGrabedCollisionIndex].contact.normal.x != 0;
        if (IsGrab && isGrabSide)
        {
            if(beGrabed.transform.position.x > transform.position.x)
            {
                rigid.AddForce(Vector2.right * 100);
            }
            else
            {
                rigid.AddForce(Vector2.left * 100);
            }
        }
        isGround = CheckGround();
        // �̵�
        if (status == Status.Death || isHit)
        {
            moveDirection = Vector2.zero;
        }
        else
        {
            if(IsGrab && isGrabSide) { moveDirection = preferDirection * Vector2.up; }
            else moveDirection = preferDirection * Vector2.right;
        }

        moveDirection.Normalize();
        if (IsGrab && moveDirection.y > 0) moveDirection.y *= 1.1f;
        if(moveDirection.magnitude > 0)
        {
            rigid.AddForce(moveDirection, ForceMode2D.Impulse);
            Vector2 speedLimitVelocity = rigid.velocity;
            speedLimitVelocity.x = Mathf.Clamp(speedLimitVelocity.x, -moveSpeed, moveSpeed);
            if(IsGrab && isGrabSide) speedLimitVelocity.y = Mathf.Clamp(speedLimitVelocity.y, -moveSpeed, moveSpeed);
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
        if(anim != null)
        {
            anim.SetFloat("speed", rigid.velocity.magnitude);
            anim.SetBool("isGround", isGround);
            anim.SetBool("isFall", rigid.velocity.y < 0);
        }
    }


    protected virtual void OnMove(InputValue value) 
    {
        Move(value.Get<Vector2>());
    }

    public virtual void Move(Vector2 direction)
    {
        direction.Normalize();
        preferDirection= direction;
    }

    protected virtual void OnJump()
    {
        Jump();
    }

    public virtual void Jump()
    {
        if(IsGrab) IsGrab= false;
        if (!isGround) return;
        rigid.AddForce(Vector2.up * jumpPower);
        isGround= false;
    }

    protected virtual void OnAttack()
    {
        Attack();
    }

    public virtual void Attack()
    {
        if(anim != null)
        {
            isAttack = true;
            anim.SetTrigger("doAttack");

        }
    }

    protected virtual void GenerateHitBox(string hitBoxName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Hitboxes/{hitBoxName}");
        GameObject inst = Instantiate(prefab, transform.position, transform.rotation);
        inst.transform.localScale = new Vector2(transform.localScale.x, inst.transform.localScale.y);
        inst.AddComponent<DamageComponent>().Initialize(this, attackDamage);
    }


    public bool CheckGround()
    {
        // �ڿ���Ÿ���� ���� ����Ʈ�� ����
        collisionList.RemoveAll(target => target.time < Time.time - coyoteTime);
        if (rigid.velocity.y == 0) return true;
        // ������ 46�� ���ϸ� ���� ������ ������ ����
        return collisionList.FindIndex(target => Vector2.Angle(target.contact.normal, Vector2.up) < 46.0f) >= 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Grabable"))
        {
            IsGrab = true;
            beGrabed = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject == beGrabed)
        {
            IsGrab= false;
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        // ������ ���� ����Ʈ ����
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        // �ݸ����� ����Ʈ ������ �޾Ƽ� contacts�� �ֱ�
        collision.GetContacts(contacts);
        // �ݸ����� ����Ʈ ������ �� �� ������Ʈ�� ���� ���� ����Ʈ ���� ã��
        ContactPoint2D myContact = System.Array.Find(contacts, target => target.otherCollider.gameObject == gameObject);

        // �̹� �ݸ�������Ʈ�� ����ִ� ���� ������ �ʵ��� �ߺ�üũ
        int collisionIndex = collisionList.FindIndex(target => target.other == collision.gameObject);
        if(collisionIndex == -1)
        {
            // �Ȱ��� ���� ���ٸ� ����Ʈ�� �߰�
            collisionList.Add(new ContactInfo(collision.gameObject, myContact, Time.time));
        }
        else 
        {
            // �̹� ��� �ִٸ� Ÿ�Ӹ� ����
            collisionList[collisionIndex] = new ContactInfo(collision.gameObject, myContact, Time.time);
        }
        
    }
}
