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
    Collider2D bodyCollider;
    DistanceJoint2D distanceJoint;
    LineRenderer chainArmPredictLineRenderer;

    GameObject grabedTarget;
    Character grabedCharacter;

    public static float coyoteTime = 0.08f;

    public Status status = Status.Normal;

    // LayerMask를 변수로 지정해두면 인스펙터창에서 관리하기 쉽다.
    [SerializeField] LayerMask chainArmPredictLayerMask;

    public Vector2 preferDirection;
    public Vector2 moveDirection;
    public Vector2 faceDirection;
    Vector2 preferReversableDashDirection;
    Vector2 preferReversableDashDirectionX;
    Vector2 preferReversableDashDirectionY;
    Vector2 chainAttackDashDirection;
    public Vector2 velocityCorrection;

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


    [SerializeField]bool isGround = false;
    public bool isHit = false;
    public bool isAttack = false;
    [SerializeField] private bool _isGrab = false;
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
    public bool isChainAttack = false;
    bool _isKnockBack = false;
    bool IsKnockBack
    {
        get => _isKnockBack;
        set
        {
            _isKnockBack= value;
            if(value == true)
            {
                Invoke(nameof(KnockBackEnd), 0.5f);
            }
        }
    }
    float tryChainArmPull;
    float chainArmJumpTime = 1f;

    GameObject chainArmPrefab;
    [SerializeField]GameObject chainArm;
    public GameObject beGrabed;
    [SerializeField]protected List<ContactInfo> collisionList = new List<ContactInfo>();

    protected override void Start()
    {
        //Time.timeScale = 0.1f;
        rigid = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        if(CompareTag("Player")) hitAnim = GetComponentsInChildren<Animator>()[1];
        distanceJoint = GetComponent<DistanceJoint2D>();
        chainArmPredictLineRenderer = GetComponent<LineRenderer>();
        base.Start();

        chainArmPrefab = Resources.Load<GameObject>($"Prefabs/Characters/ChainHand");

        faceDirection = Vector2.right;

    }
    protected override void Update()
    {
        if(CompareTag("Player")) PredictChainArmTrajectory();
        if(isChainAttack)
        {
            if(Input.GetMouseButtonUp(0))
            {
                ChainAttackEnd();
            }
        }
        base.Update();
    }

    private void FixedUpdate()
    {
        if (rigid.bodyType == RigidbodyType2D.Static) return;
        if(grabedTarget != null)
        {
            transform.position = grabedTarget.GetComponent<Collider2D>().bounds.center;

        }
        if(CompareTag("Player"))
        {
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
        if (IsGrab)
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
                    if (IsGrab) moveDirection = preferDirection * Vector2.up;
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
            if((IsGrab) || flyable) speedLimitVelocity.y = Mathf.Clamp(speedLimitVelocity.y, -moveSpeed, moveSpeed);
            else if (chainArmJumpTime < 1.1f) speedLimitVelocity.y = Mathf.Clamp(speedLimitVelocity.y, -moveSpeed * 10, moveSpeed);
            rigid.velocity = speedLimitVelocity;
            faceDirection = rigid.velocity * Vector2.right;
            if (rigid.velocity.magnitude <= 0.01f) faceDirection = Vector2.right;
            faceDirection.Normalize();
        }
        else
        {
            if(!IsKnockBack)rigid.velocity *= Vector2.up;
            if(!IsGrab)rigid.velocity += velocityCorrection;
            else rigid.velocity += velocityCorrection * Vector2.right;
        }
        if(transform.localScale.x * faceDirection.x < 0)
        {
            transform.localScale *= new Vector2(-1, 1);
        }

        if(flyable && !IsBreak)
        {
            transform.Rotate(0, 0, -moveDirection.x * moveSpeed * Time.fixedDeltaTime * 50);
            if (transform.eulerAngles.z > 30 && transform.eulerAngles.z < 180) transform.eulerAngles = new Vector3(0, 0, 30);
            if (transform.eulerAngles.z > 180 && transform.eulerAngles.z < 330) transform.eulerAngles = new Vector3(0, 0, 330);
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

    void PredictChainArmTrajectory()
    {
        if(chainArm == null)
        {
            chainArmPredictLineRenderer.enabled = true;
            Vector3 center = bodyCollider.bounds.center;
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - center;
            direction.Normalize();
            Ray predictRay = new Ray(center, direction * 30);
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = false;
            filter.SetLayerMask(chainArmPredictLayerMask);
            RaycastHit2D[] hits = new RaycastHit2D[20];
            int hitAmount = Physics2D.Raycast(predictRay.origin, predictRay.direction, filter, hits);
            if(hitAmount > 0)
            {
                chainArmPredictLineRenderer.SetPosition(0, center);
                chainArmPredictLineRenderer.SetPosition(1, hits[0].point);
                // hits[0].transform.gameObject.layer는 9가 반환 되고 LayerMask.GetMask("Enemy")는 2^9으로 반환되기 때문에
                // left shift를 통해 (1 << x) 비교해주어야 한다.
                // (layer에서 tag로 교체)
                if (hits[0].transform.gameObject.CompareTag("Enemy")|| hits[0].transform.gameObject.CompareTag("Justice"))
                {
                    
                    chainArmPredictLineRenderer.startColor = Color.red;
                    chainArmPredictLineRenderer.endColor= Color.red;

                }
                else
                {
                    chainArmPredictLineRenderer.startColor = Color.cyan;
                    chainArmPredictLineRenderer.endColor= Color.cyan;
                }
            }
            else
            {
                chainArmPredictLineRenderer.enabled=false;

            }
        }
        else
        {
            chainArmPredictLineRenderer.enabled=false;
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
        if (isChainAttack && grabedCharacter != null) grabedCharacter.Move(value.Get<Vector2>());
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
        velocityCorrection *= Vector2.right;
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
        inst.GetComponent<ChainArm>().userChar = this;
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
        rigid.AddForce(Vector2.up * jumpPower);
    }


    public void ChainAttack(Character target)
    {
        DestroyChainArm();
        grabedTarget = target.gameObject;
        grabedCharacter = target;
        if(Input.GetMouseButton(0)) 
        { 
            isChainAttack= true;
            target.status = Status.Grabed;
            Invoke("ChainAttackEnd", 3f);
        }
        else
        {
            Invoke("ChainAttackEnd", 0.1f);
        }
    }

    public void ChainAttack(GameObject target)
    {
        DestroyChainArm();
        grabedTarget = target;
        if(Input.GetMouseButton(0))
        {
            isChainAttack = true;
        }
        else
        {
            ChainAttackEnd();
        }
    }

    public void ChainAttack(ExecutorCore executor)
    {
        DestroyChainArm();
        executor.TakeDamage(this, attackDamage, transform.position);
    }

    public void ChainAttack(JucticeCoreBreakable jucticeCore)
    {
        DestroyChainArm();
        grabedTarget = jucticeCore.gameObject;
        jucticeCore.TakeDamage(this, attackDamage, transform.position);
        Invoke("ChainAttackEnd", 0.1f);
    }

    void ChainAttackEnd()
    {
        if (grabedTarget != null)
        {
            if(grabedCharacter != null)
            {
                grabedCharacter.status = Status.Normal;
                grabedCharacter.TakeDamage(this, attackDamage, transform.position);
                grabedCharacter = null;
            }
            else
            {
                // patrolBlock
                if(grabedTarget.TryGetComponent<PatrolBlock>(out PatrolBlock patrol))
                {
                    patrol.movable = false;
                    if(patrol.isGoingBack)
                    {
                        patrol.transform.position = patrol.endPoint.position;

                    }
                    else
                    {
                        patrol.transform.position = patrol.startPoint.position;

                    }
                }
                else
                {
                    // justiceCore
                    
                }
            }
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
        GameObject inst = Instantiate(prefab, transform);
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
            int contactIndex = System.Array.FindIndex(collision.contacts, target => target.otherCollider.gameObject == gameObject);
            if(Vector2.Angle(collision.GetContact(contactIndex).normal, Vector2.up) == 90) IsGrab = true;
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

    public override int TakeDamage(Breakable from, int damage, Vector3 hitPoint)
    {
        if (CompareTag("Player"))
        {
            // 넉백
            if(hitPoint != null)
            {
                Vector2 knockBackDirection = bodyCollider.bounds.center - hitPoint;
                knockBackDirection.Normalize();
                IsKnockBack= true;
                rigid.AddForce(knockBackDirection * 3, ForceMode2D.Impulse);
            }
            
            // 플레이어 히트 이펙트
            isReversalDashableX= true;
            isReversalDashableY= true;
            hitAnim.SetTrigger("doHit");
            if(chainArm != null) DestroyChainArm();
            IsGrab = false;
            Time.timeScale = 0.2f;
            Invoke("ReversalDash", 0.15f);
            Invoke("ReversalDashEnd", 1f);
            
        } else if(CompareTag("Justice"))
        {
            GetComponent<AIJustice>().bodyCollider.enabled= false;
            anim.SetTrigger("doHit");
        }
        return base.TakeDamage(from, damage, hitPoint);
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

    void KnockBackEnd()
    {
        IsKnockBack = false;
    }

}
