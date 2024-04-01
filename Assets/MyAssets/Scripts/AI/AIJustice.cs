using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIJustice : AIBase
{
    private void Start()
    {
        controlledCharacter = GetComponent<Character>();
    }

    protected override void FixedUpdate()
    {
        target = FindTarget();
        if (target != null)
        {
            Vector2 targetDirection = target.transform.position - controlledCharacter.transform.position;
            controlledCharacter.faceDirection = targetDirection * Vector2.left;
            //if(!controlledCharacter.isAttack)
            {
                //controlledCharacter.faceDirection = targetDirection * Vector2.left;
                //transform.rotation = Quaternion.Euler(0, 0, 0);

            }
            //else
            {
                //controlledCharacter.faceDirection = targetDirection * new Vector2(-1, 1);
                if(targetDirection.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x)*Mathf.Rad2Deg + 180);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x)*Mathf.Rad2Deg);

                }

            }
            controlledCharacter.faceDirection.Normalize();
            if (CheckAttackable())
            {
                // 공격 범위 안에 있다면 공격
                controlledCharacter.preferDirection = Vector2.zero;
               // controlledCharacter.Attack();
            }
            else if (controlledCharacter.isAttack)
            {
                // 공격 중에는 움직이지 않게
                controlledCharacter.preferDirection = Vector2.zero;
            }

        }
        else
        {
            controlledCharacter.preferDirection = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if((collision.CompareTag("Player") || collision.CompareTag("ChainArm")) && !controlledCharacter.isAttack)
        {
            controlledCharacter.anim.SetTrigger("doAttack");

        }
    }

}
