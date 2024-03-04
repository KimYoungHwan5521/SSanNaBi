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
            if (CheckAttackable()) controlledCharacter.Attack();
            controlledCharacter.preferDirection = target.transform.position - transform.position;
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
                if(controlledCharacter.CheckEnemy(Btarget))
                {
                    return Btarget;
                }
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
