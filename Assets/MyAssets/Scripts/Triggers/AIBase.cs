using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class AIBase : MonoBehaviour
{
    Character controlledCharacter;
    Breakable target;
    public float detectRange;
    public float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<UnityEngine.InputSystem.PlayerInput>())
        {
            Destroy(this);
        }
        controlledCharacter = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        target = FindTarget();
        if(target != null) 
        {
            if (CheckAttackable())
            {
                controlledCharacter.preferDirection = Vector2.zero;
                controlledCharacter.faceDirection = target.transform.position.x - transform.position.x < 0 ? Vector2.left: Vector2.right;
                controlledCharacter.Attack();
            }
            else
            {
                controlledCharacter.preferDirection = target.transform.position - transform.position;
            }

        }
        else
        {
            controlledCharacter.preferDirection = Vector2.zero;
        }
    }

    protected bool CheckAttackable()
    {
        if (Vector2.Distance(target.transform.position, transform.position) < attackRange) return true;
        return false;
    }

    protected Breakable FindTarget()
    {
        RaycastHit2D[] raycasts = Physics2D.CircleCastAll(transform.position, detectRange, Vector2.zero);
        foreach(RaycastHit2D raycast in raycasts)
        {
            if(raycast.collider.TryGetComponent(out Breakable Btarget))
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
        Vector2 direction = new Vector2(targetBoundsCenter.x - spawnPosition.x, targetBoundsCenter.y - spawnPosition.y);
        direction.Normalize();
        GameObject inst = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x)));
        inst.AddComponent<DamageComponent>().Initialize(controlledCharacter, controlledCharacter.attackDamage);
        inst.GetComponentInChildren<Rigidbody2D>().velocity = direction * 10;
        Debug.Log($"targetBoundsCenter : {targetBoundsCenter}, spawnPosition : {spawnPosition}, direction : {direction}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
