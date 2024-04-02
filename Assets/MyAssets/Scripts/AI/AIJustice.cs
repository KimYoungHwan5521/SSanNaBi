using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIJustice : AIBase
{
    public SpriteRenderer dashRangeSR;
    public float dashRange;

    public float dashAttackCoolTime;
    float curDashAttackCoolTime;
    bool dashAttackReady;
    public float dashCharge;
    float curDashCharge;
    Vector3 dashPosition;
    Vector3 dashVector;
    bool dashAttack;
    public float dashAfterDelay;
    float curDashAfterDelay;
    bool dashAfter;

    private void Start()
    {
        controlledCharacter = GetComponent<Character>();

        curDashAttackCoolTime = dashAttackCoolTime;
        curDashCharge= dashCharge;
        curDashAfterDelay= dashAfterDelay;
        if(dashRangeSR!= null)
        {
            dashRangeSR.transform.localScale = new Vector3(dashRange, 4, 1);
        }
    }

    protected override void FixedUpdate()
    {
        target = FindTarget();
        if (target != null)
        {
            
            Vector3 targetDirection = target.transform.position - controlledCharacter.transform.position;
            // 타겟 바라보기
            if (!controlledCharacter.isAttack && !dashAttack && !dashAfter)
            {
                controlledCharacter.faceDirection = targetDirection * Vector2.left;
                controlledCharacter.faceDirection.Normalize();
                if(targetDirection.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x)*Mathf.Rad2Deg + 180);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x)*Mathf.Rad2Deg);

                }

            }

            targetDirection.Normalize();

            if (curDashAttackCoolTime < 0 && !dashAttackReady)
            {
                dashAttackReady = true;
                if (dashRangeSR != null) { dashRangeSR.enabled = true; };
            }

            if(dashAttackReady)
            {
                if(dashAttack)
                {
                    if((10 * controlledCharacter.moveSpeed * Time.fixedDeltaTime * dashVector).magnitude < Vector2.Distance(transform.position, dashPosition))
                    {
                        transform.parent.position += 10 * controlledCharacter.moveSpeed * Time.fixedDeltaTime * dashVector;
                    }
                    else
                    {
                        transform.parent.position = dashPosition;
                        dashAttack = false;
                        curDashCharge = dashCharge;
                        curDashAttackCoolTime = dashAttackCoolTime;
                        dashAttackReady = false;
                        dashAfter = true;
                    }

                }
                else
                {
                    if(curDashCharge < 0)
                    {
                        dashVector = target.transform.position - transform.position;
                        dashVector.Normalize();
                        dashPosition = Vector2.Distance(target.transform.position, transform.position) < dashRange - 6 ? target.transform.position : transform.position + dashVector * (dashRange - 6);
                        if (dashRangeSR != null) { dashRangeSR.enabled = false; };
                        dashAttack = true;
                        controlledCharacter.anim.SetTrigger("doAttack");
                    }
                    else
                    {
                        curDashCharge -= Time.fixedDeltaTime;
                    }

                }
            }
            else if(dashAfter)
            {
                if(curDashAfterDelay < 0)
                {
                    dashAfter = false;
                    curDashAfterDelay = dashAfterDelay;
                }
                curDashAfterDelay -= Time.fixedDeltaTime;
            }
            else
            {
                transform.parent.position += controlledCharacter.moveSpeed * Time.fixedDeltaTime * targetDirection;
                curDashAttackCoolTime -= Time.fixedDeltaTime;

                // 공격 중에는 움직이지 않게
                if (controlledCharacter.isAttack)
                {
                    controlledCharacter.preferDirection = Vector2.zero;
                }

            }



        }
        else
        {
            controlledCharacter.preferDirection = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if((collision.CompareTag("Player") || collision.CompareTag("ChainArm")) && !controlledCharacter.isAttack && !dashAttackReady)
        {
            controlledCharacter.isAttack= true;
            controlledCharacter.anim.SetTrigger("doAttack");

        }
    }

}
