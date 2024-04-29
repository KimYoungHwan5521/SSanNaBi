using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainArm : MonoBehaviour
{
    Rigidbody2D rigid;

    LineRenderer lineRenderer;
    public Transform user;
    public Character userChar;
    public bool isChainArmGrab = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, user.position) > userChar.chainArmMaxDistance)
        {
            userChar.DestroyChainArm();
        }

        lineRenderer.SetPosition(0, user.position);
        lineRenderer.SetPosition(1, transform.position);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        // 공격 받으면 끊어짐
        if(collision.CompareTag("HitBox"))
        {
            isChainArmGrab = false;
            userChar.DestroyChainArm();
        }

        // 벽 잡으면 붙음
        if (collision.gameObject.CompareTag("Grabable"))
        {
            if(collision.TryGetComponent(out Rigidbody2D collisionRigid))
            {
                rigid.velocity= collisionRigid.velocity;

            }
            else rigid.velocity= Vector3.zero;
            isChainArmGrab= true;
            user.GetComponent<DistanceJoint2D>().distance = Vector2.Distance(user.position, transform.position);
        }
        else if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Justice")) && collision.gameObject.GetComponent<Breakable>().curInvincibleTime <= 0)
        {
            if (collision.TryGetComponent(out Character character))
            {
                userChar.ChainAttack(character);
            }
            else if (collision.TryGetComponent(out ExecutorCore executor))
            {
                userChar.ChainAttack(executor);
            }
            else if (collision.TryGetComponent(out JucticeCoreBreakable justiceCore))
            {
                userChar.ChainAttack(justiceCore);
            }


        }
        else if (collision.gameObject.CompareTag("DoorButton"))
        {
            isChainArmGrab = false;
            userChar.DestroyChainArm();
            collision.gameObject.GetComponent<DoorButton>().ButtonOn();
        }
        else if (collision.gameObject.CompareTag("ChainGrabable"))
        {
            userChar.ChainAttack(collision.gameObject);
            collision.gameObject.GetComponent<PatrolBlock>().movable = true;
        }
        else if(collision.gameObject.CompareTag("Untagged"))
        {
            isChainArmGrab= false;
            userChar.DestroyChainArm();
        }
    }



}
