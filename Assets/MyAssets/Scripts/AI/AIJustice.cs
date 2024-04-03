using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIJustice : AIBase
{
    public SpriteRenderer dashRangeSR;
    public SpriteRenderer circularAttackRangeSR;
    public float dashRange;
    public JusticeCore justiceCore;
    SpriteRenderer[] coreSprites;

    public Collider2D bodyCollider;

    Vector3 targetDirection;
    Quaternion rememberRotation;
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

    public bool isWeak;
    bool beHit;

    float readyToNextPhase = 3f;
    float curReadyToNextPhase;
    bool nextPhase;

    public float circularAttackReady;
    float curCircularAttackReady;
    bool isCircularAttackReady;
    public float circularAttackDelay;
    float curCircularAttackDelay;

    private void Start()
    {
        controlledCharacter = GetComponent<Character>();
        bodyCollider= GetComponent<Collider2D>();
        coreSprites = justiceCore.GetComponentsInChildren<SpriteRenderer>();

        curDashAttackCoolTime = dashAttackCoolTime;
        curDashCharge= dashCharge;
        curDashAfterDelay= dashAfterDelay;
        curReadyToNextPhase= readyToNextPhase;
        curCircularAttackReady= circularAttackReady;
        curCircularAttackDelay= circularAttackDelay;
        if(dashRangeSR!= null)
        {
            dashRangeSR.transform.localScale = new Vector3(dashRange, 4, 1);
        }
        
    }

    private void LateUpdate()
    {
        target = FindTarget();
        if (target != null)
        {
            if (!isWeak)
            {
                if (!nextPhase)
                {
                    targetDirection = target.transform.position - controlledCharacter.transform.position;
                    // Ÿ�� �ٶ󺸱�
                    if (!controlledCharacter.isAttack && !dashAttack && !dashAfter && !beHit && !isCircularAttackReady)
                    {
                        controlledCharacter.faceDirection = targetDirection * Vector2.left;
                        controlledCharacter.faceDirection.Normalize();
                        if (targetDirection.x < 0)
                        {
                            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 180);
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg);

                        }
                        rememberRotation = transform.rotation;
                    }
                    else
                    {
                        transform.rotation = rememberRotation;
                    }
                    targetDirection.Normalize();

                }
            }
        }
    }
    
    protected override void FixedUpdate()
    {
        target = FindTarget();
        if (target != null)
        {
            if(!isWeak)
            {
                if (!nextPhase)
                {
                    if (curDashAttackCoolTime < 0 && !dashAttackReady)
                    {
                        dashAttackReady = true;
                        if (dashRangeSR != null) { dashRangeSR.enabled = true; };
                    }

                    if(!beHit)
                    {
                        if (dashAttackReady)
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
                                    if (!justiceCore.isCoreActivated) justiceCore.CoreActivate();
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
                            // �̵�
                            transform.parent.position += controlledCharacter.moveSpeed * Time.fixedDeltaTime * targetDirection;
                            curDashAttackCoolTime -= Time.fixedDeltaTime;
                              
                        }

                    }

                }
                else
                {
                    // nextPhase
                    if(curCircularAttackReady < 0)
                    {
                        if(!isCircularAttackReady)
                        {
                            transform.parent.position = target.transform.position;
                            circularAttackRangeSR.enabled = true;
                            isCircularAttackReady= true;
                        }
                        if(curCircularAttackDelay < 0)
                        {
                            circularAttackRangeSR.enabled = false;
                            for (int i = 0; i < coreSprites.Length; i++)
                            {
                                if(i >= justiceCore.activateCore * 2)coreSprites[i].enabled = true;
                            }
                            controlledCharacter.anim.SetBool("isHide", false);
                            controlledCharacter.anim.SetTrigger("doCircularAttack");
                        }
                        else
                        {
                            curCircularAttackDelay-= Time.fixedDeltaTime;
                        }

                    }
                    else
                    {
                        curCircularAttackReady-= Time.fixedDeltaTime;
                    }
                }
                
                if(beHit)
                {
                    if (curReadyToNextPhase < 0)
                    {
                        beHit = false;
                        nextPhase = true;
                        for(int i = 0;i<coreSprites.Length; i++)
                        {
                            coreSprites[i].enabled = false;
                        }

                        controlledCharacter.anim.SetBool("isHide", true);
                        curReadyToNextPhase = readyToNextPhase;
                    }
                    else
                    {
                        curReadyToNextPhase -= Time.fixedDeltaTime;
                    }

                }
            }



        }
    }
    
    void BeHit()
    {
        beHit= true;
        isWeak = false;
        curDashAfterDelay = dashAfterDelay;
        curDashAttackCoolTime= dashAttackCoolTime;
        curDashCharge = dashCharge;
        controlledCharacter.anim.SetBool("isWeak", false);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if((collision.CompareTag("Player") || (collision.CompareTag("ChainArm") && !justiceCore.isCoreActivated)) && !controlledCharacter.isAttack && !dashAttackReady && !isWeak && !beHit && !nextPhase)
        {
            controlledCharacter.isAttack= true;
            controlledCharacter.anim.SetTrigger("doAttack");

        }
    }

    public void JusticeCoreHit()
    {
        isWeak = true;
        bodyCollider.enabled = true;
        dashRangeSR.enabled = false;
        controlledCharacter.anim.SetBool("isWeak", true);
        controlledCharacter.anim.ResetTrigger("doAttack");
        controlledCharacter.anim.StopPlayback();
    }

    public void CircularAttackEnd()
    {
        controlledCharacter.anim.ResetTrigger("doCircularAttack");
        nextPhase = false;
        isCircularAttackReady= false;
        curCircularAttackDelay = circularAttackDelay;
        curCircularAttackReady = circularAttackReady;
    }
}
