using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Normal, Fixed, Patrol }

[RequireComponent(typeof(Character))]
public class AIBase : MonoBehaviour
{
    public Character controlledCharacter;
    [SerializeField]protected Breakable target;
    protected CapsuleCollider2D capsule;
    protected Rigidbody2D rigid;

    public MoveType moveType;

    public float targetDetectRange;
    public float attackRange;

    public float obstacleDetectRange = 1f;
    public float cliffDetectRange = 3f;

    public bool jumpable;


    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<UnityEngine.InputSystem.PlayerInput>())
        {
            Destroy(this);
        }
        controlledCharacter = GetComponent<Character>();
        capsule= GetComponent<CapsuleCollider2D>();
        rigid= GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (controlledCharacter.status == Status.Grabed) return;
        target = FindTarget();
        // AI�� ���߿��� ���� ���ٲٰ�
        if(controlledCharacter.CheckGround())
        {
            if(target != null) 
            {
                Vector2 targetDirection = target.transform.position - controlledCharacter.transform.position;
                controlledCharacter.faceDirection = targetDirection * Vector2.right;
                controlledCharacter.faceDirection.Normalize();
                if (CheckAttackable())
                {
                    // ���� ���� �ȿ� �ִٸ� ����
                    controlledCharacter.preferDirection = Vector2.zero;
                    controlledCharacter.faceDirection = target.transform.position.x - transform.position.x < 0 ? Vector2.left: Vector2.right;
                    controlledCharacter.Attack();
                }
                else if(controlledCharacter.isAttack)
                {
                    // ���� �߿��� �������� �ʰ�
                    controlledCharacter.preferDirection = Vector2.zero;
                }
                else if(moveType != MoveType.Fixed)
                {
                    // ���� ���� ���̶�� Ÿ�ٿ��� �̵�
                    // �����̸� ������ĳ��Ʈ�� ���ؼ� ������ ���� ������ ����.
                    if(CheckCliff() && targetDirection.y > -1)
                    {
                        Vector3 correctionVector = controlledCharacter.transform.position + capsule.bounds.extents.x * Vector3.up;
                        RaycastHit2D hit = correctionVector.Parabolacast2D(controlledCharacter.faceDirection * controlledCharacter.moveSpeed + controlledCharacter.jumpPower * Vector2.up * 0.03f, Physics2D.gravity, 3f, 10);
                        // cast�� ���� ���� ������ �ƴ϶� õ���� ���� �����Ƿ� hit.normal�� Vector.up �������� 45�� �̳����� üũ
                        // ������ ���
                        if(hit.collider!= null && Vector2.Angle(Vector2.up, hit.normal) < 45)
                        {
                            if(moveType == MoveType.Normal && jumpable)
                            {
                                controlledCharacter.Jump();

                            }
                        }
                        else
                        {
                            controlledCharacter.preferDirection = Vector2.zero;
                        }
                    }
                    else
                    {
                        // Ÿ���� �ξ� �Ʒ����̸� �����̾ ���� ��������.
                        if(moveType == MoveType.Normal)
                        {
                            controlledCharacter.preferDirection = target.transform.position - transform.position;
                        }

                        if(CheckObstacle())
                        {
                            if(moveType == MoveType.Normal && jumpable)
                            {
                                controlledCharacter.Jump();
                            }
                        }

                    }
                }

            }
            else
            {
                if(moveType == MoveType.Normal)
                {
                    controlledCharacter.preferDirection = Vector2.zero;
                }
                else if(moveType == MoveType.Patrol)
                {
                    //Debug.Log(CheckObstacle());
                    if(CheckObstacle() || CheckCliff())
                    {
                        controlledCharacter.faceDirection *= -1;
                    }
                    controlledCharacter.preferDirection = controlledCharacter.faceDirection;
                }
            }
        }
    }

    protected bool CheckCliff()
    {
        Vector3 stepableHeight = capsule.bounds.center + Vector3.down * (capsule.bounds.extents.y - capsule.bounds.extents.x) + Vector3.right * controlledCharacter.faceDirection.x;
        Ray cliffRay = new Ray(stepableHeight, Physics2D.gravity);
        Debug.DrawRay(cliffRay.origin, cliffRay.direction * cliffDetectRange);

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(LayerMask.GetMask("Default"));
        RaycastHit2D[] hits = new RaycastHit2D[20];
        int hitAmount = Physics2D.Raycast(cliffRay.origin, cliffRay.direction, filter, hits, cliffDetectRange);
        if (hitAmount == 0) return true;
        return false;
    }

    protected bool CheckObstacle()
    {
        Vector3 stepableHeight = capsule.bounds.center + Vector3.down * (capsule.bounds.extents.y - capsule.bounds.extents.x);
        Ray stepRay = new Ray(stepableHeight, controlledCharacter.moveDirection);
        Debug.DrawRay(stepRay.origin, stepRay.direction * obstacleDetectRange);

        // ���͸� ���� �̵��� ���ص��� �ʴ� ������Ʈ�� ���͸�
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;
        filter.SetLayerMask(LayerMask.GetMask("Default"));
        RaycastHit2D[] hits = new RaycastHit2D[20];
        int hitAmount = Physics2D.Raycast(stepRay.origin, stepRay.direction, filter, hits, obstacleDetectRange);
        if(hitAmount > 0)
        {
            return true;
        }
        return false;
    }

    protected bool CheckAttackable()
    {
        if (Vector2.Distance(target.transform.position, transform.position) < attackRange) return true;
        return false;
    }

    protected Breakable FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetDetectRange, Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.TryGetComponent(out Breakable Btarget))
            {
                if(Btarget.IsBreak == false && controlledCharacter.CheckEnemy(Btarget))
                {
                    return Btarget;
                }
            }
        }
        return null;
    }

    protected virtual void GenerateProjectile(string projectileName)
    {
        if(target == null) return;
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Projectiles/{projectileName}");
        Vector2 spawnPosition = transform.childCount > 0 ? GetComponentsInChildren<Transform>()[1].position : transform.position;
        Vector2 targetBoundsCenter = target.GetComponent<Collider2D>().bounds.center;
        Vector2 direction = new Vector2((targetBoundsCenter.x - spawnPosition.x) * Random.Range(0.9f, 1.1f), (targetBoundsCenter.y - spawnPosition.y) * Random.Range(0.9f, 1.1f));
        direction.Normalize();
        GameObject inst = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x)));
        inst.AddComponent<DamageComponent>().Initialize(controlledCharacter, controlledCharacter.attackDamage, true);
        inst.GetComponent<Rigidbody2D>().velocity = direction * 30;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, targetDetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }

}
