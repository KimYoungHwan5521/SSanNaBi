using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Status { Normal, Death, Grabed }

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
// [RequireComponent(typeof(Animator))]
public class Character : Breakable
{
    public Animator anim;
    Animator hitAnim;
    Rigidbody2D rigid;
    Collider2D bodyCollider;
    DistanceJoint2D distanceJoint;

    Character grabedTarget;

    public float coyoteTime = 0.02f;

    public Status status = Status.Normal;

    public Vector2 preferDirection;
    public Vector2 moveDirection;
    public Vector2 faceDirection;
    Vector2 preferReversableDashDirection;
    Vector2 preferReversableDashDirectionX;
    Vector2 preferReversableDashDirectionY;
    Vector2 chainAttackDashDirection;

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
    public float chainArmSpeed;

    public float chainArmMaxDistance;

    public bool flyable;
    [SerializeField]bool isGround = false;
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
    bool isReversalDashableX;
    bool isReversalDashableY;
    bool isChainAttack = false;
    float tryChainArmPull;
    float chainArmJumpTime = 1f;

    GameObject chainArmPrefab;
    [SerializeField]GameObject chainArm;
    public GameObject beGrabed;
    protected List<ContactInfo> collisionList = new List<ContactInfo>();

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        if(CompareTag("Player")) hitAnim = GetComponentsInChildren<Animator>()[1];
        distanceJoint = GetComponent<DistanceJoint2D>();

        chainArmPrefab = Resources.Load<GameObject>($"Prefabs/Character/ChainHand");

    }
    private void Update()
    {
        if(isChainAttack)
        {
            if(Input.GetMouseButtonUp(0))
            {
                ChainAttackEnd();
            }
        }
    }

    private void FixedUpdate()
    {
        if(grabedTarget != null)
        {
            transform.position = grabedTarget.GetComponent<Collider2D>().bounds.center;

        }
        if(isChainAttack)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
            bodyCollider.isTrigger = true;
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            bodyCollider.isTrigger = false;

        }
        if(chainArm != null)
        {
            if(chainArm.GetComponent<ChainArm>().isChainArmGrab)
            {
                if(isGround || IsGrab)
                {
                    distanceJoint.enabled = false;
                }
                else
                {
                    distanceJoint.enabled = true;
                }
                if(tryChainArmPull > 0)
                {
                    distanceJoint.enabled = true;
                    distanceJoint.distance = Mathf.Clamp(distanceJoint.distance - 0.3f, 0.5f, chainArmMaxDistance);
                }
            }

        }
        // 그랩 가능한 땅을 왼쪽 혹은 오른쪽에서 잡았을 때
        int beGrabedCollisionIndex = collisionList.FindIndex(target => target.other == beGrabed);
        // 노말의 x가 0이면 위나 아래에서 붙은것.
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
        if (isGround) chainArmJumpTime = 1f;
        else if(chainArmJumpTime > 1) chainArmJumpTime -= Time.fixedDeltaTime;
        // 이동
        if (status == Status.Death)
        {
            moveDirection = Vector2.zero;
        }
        else
        {
            if(preferReversableDashDirection != Vector2.zero) moveDirection = preferReversableDashDirection;
            else if(chainAttackDashDirection != Vector2.zero) moveDirection = chainAttackDashDirection;
            else
            {
                if (flyable)
                {
                    moveDirection = preferDirection;
                }
                else
                {
                    if (IsGrab && isGrabSide) moveDirection = preferDirection * Vector2.up;
                    else moveDirection = preferDirection * Vector2.right;

                }

            }
        }

        moveDirection.Normalize();
        // 벽을 오를때 중력때문에 너무 느려서 보정치를 줌
        if (IsGrab && moveDirection.y > 0) moveDirection.y *= 1.1f;
        if(moveDirection.magnitude > 0 || (chainArm != null && !isGround))
        {
            rigid.AddForce(moveDirection, ForceMode2D.Impulse);
            Vector2 speedLimitVelocity = rigid.velocity;
            if(((isGround || chainArm == null || !chainArm.GetComponent<ChainArm>().isChainArmGrab)) && preferReversableDashDirection == Vector2.zero && chainAttackDashDirection == Vector2.zero)
            {
                speedLimitVelocity.x = Mathf.Clamp(speedLimitVelocity.x, -moveSpeed * chainArmJumpTime, moveSpeed * chainArmJumpTime);
            }
            if(IsGrab && isGrabSide) speedLimitVelocity.y = Mathf.Clamp(speedLimitVelocity.y, -moveSpeed, moveSpeed);
            else if (chainArmJumpTime < 1.1f) speedLimitVelocity.y = Mathf.Clamp(speedLimitVelocity.y, -moveSpeed * 10, moveSpeed);
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
            if(status == Status.Grabed) 
            {
                anim.SetTrigger("doHit");
            }
            else
            {
                anim.SetFloat("speed", rigid.velocity.magnitude);
                anim.SetBool("isGround", isGround);
                anim.SetBool("isFall", rigid.velocity.y < 0);
            }
        }
    }

    protected void OnReversalDash(InputValue value)
    {
        if(isReversalDashableX || isReversalDashableY) ReversalDash(value.Get<Vector2>());
    }

    public void ReversalDash(Vector2 value)
    {
        if(value == Vector2.left || value == Vector2.right)
        {
            preferReversableDashDirectionX = value;
            isReversalDashableX = false;
        }else if(value == Vector2.up || value == Vector2.down)
        {
            preferReversableDashDirectionY = value;
            isReversalDashableY = false;
        }
    }


    protected virtual void OnMove(InputValue value)
    {
        if (isChainAttack) grabedTarget.Move(value.Get<Vector2>());
        Move(value.Get<Vector2>());
    }

    public virtual void Move(Vector2 direction)
    {
        direction.Normalize();
        preferDirection= direction;
    }

    protected void OnChainArmPull(InputValue value)
    {
        if(chainArm != null) ChainArmPull(value.Get<float>());

    }

    public void ChainArmPull(float value)
    {
        tryChainArmPull = value;

    }

    protected virtual void OnJump()
    {
        if(chainArm == null) Jump();
        
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

    protected void OnChainArmThrow()
    {
        if(chainArm == null)
        {
            ChainArmThrow(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        }
        else
        {
            tryChainArmPull = 0;
            DestroyChainArm();
        }
    }

    public void ChainArmThrow(Vector2 throwVector)
    {
        GameObject inst = Instantiate(chainArmPrefab, transform.position, transform.rotation);
        chainArm = inst;
        throwVector.Normalize();
        inst.GetComponent<ChainArm>().user = transform;
        Rigidbody2D instRigid = inst.GetComponent<Rigidbody2D>();
        instRigid.velocity = throwVector * chainArmSpeed;
        distanceJoint.connectedBody = instRigid;
    }

    public void OnChainArmJump()
    {
        if(chainArm != null && chainArm.GetComponent<ChainArm>().isChainArmGrab) ChainArmJump();
    }

    public void ChainArmJump()
    {
        DestroyChainArm();
        chainArmJumpTime = 3f;
        rigid.AddForce(moveDirection * jumpPower);
    }


    public void ChainAttack(Character target)
    {
        DestroyChainArm();
        grabedTarget = target;
        if(Input.GetMouseButton(0)) 
        { 
            isChainAttack= true;
            grabedTarget.status = Status.Grabed;
            Invoke("ChainAttackEnd", 3f);
        }
        else
        {
            Invoke("ChainAttackEnd", 0.1f);
        }
        //transform.position = chainArm.transform.position;
    }
    void ChainAttackEnd()
    {
        if (grabedTarget != null)
        {
            grabedTarget.TakeDamage(this, attackDamage);
            grabedTarget= null;
            isChainAttack= false;
            chainAttackDashDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Invoke("ChainAttackDashEnd", 0.5f);
        }

    }

    void ChainAttackDashEnd()
    {
        chainAttackDashDirection = Vector2.zero;
    }

    public void DestroyChainArm()
    {
        distanceJoint.enabled = false;
        Destroy(chainArm);
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
        // 코요테타임이 지난 콘택트는 삭제
        collisionList.RemoveAll(target => target.time < Time.time - coyoteTime);
        if (rigid.velocity.y == 0) return true;
        // 각도가 46도 이하면 점프 가능한 땅으로 간주
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

    public override int TakeDamage(Breakable from, int damage)
    {
        if (CompareTag("Player"))
        {
            // 플레이어 히트 이펙트
            isReversalDashableX= true;
            isReversalDashableY= true;
            hitAnim.SetTrigger("doHit");
            if(chainArm != null) DestroyChainArm();
            IsGrab = false;
            Time.timeScale = 0.1f;
            Invoke("ReversalDash", 0.15f);
            Invoke("ReversalDashEnd", 1f);
        }
        return base.TakeDamage(from, damage);
    }

    void ReversalDash()
    {
        Time.timeScale = 1f;
        preferReversableDashDirection += preferReversableDashDirectionX + preferReversableDashDirectionY;
        isReversalDashableX= false;
        isReversalDashableY= false;
    }

    void ReversalDashEnd()
    {
        preferReversableDashDirection = Vector2.zero;

    }

}
