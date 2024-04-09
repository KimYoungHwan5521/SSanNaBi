using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainArm : MonoBehaviour
{
    Vector2 grabForce = Vector2.zero;
    LineRenderer lineRenderer;
    public Transform user;
    public Character userChar;
    public bool isChainArmGrab = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, user.position) > userChar.chainArmMaxDistance)
        {
            userChar.DestroyChainArm();
        }
        if (grabForce != Vector2.zero)
        {
            isChainArmGrab= true;
            GetComponent<Rigidbody2D>().AddForce(grabForce * 1000);
        }

        lineRenderer.SetPosition(0, user.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Grabable"))
        {
            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            int contactIndex = System.Array.FindIndex(contacts, target => target.otherCollider.gameObject == gameObject);
            // 잡은 콘택트 노말의 반대방향으로 잡아당기는 힘이 작용하게
            grabForce = -contacts[contactIndex].normal;

            user.GetComponent<DistanceJoint2D>().distance = Vector2.Distance(user.position, transform.position);
        }
        else if((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Justice")) && collision.gameObject.GetComponent<Breakable>().curInvincibleTime < 0)
        {
            if(collision.collider.TryGetComponent(out Character character))
            {
                userChar.ChainAttack(character);
            }
            else if(collision.collider.TryGetComponent(out ExecutorCore executor))
            {
                userChar.ChainAttack(executor);
            }
            else
            {
                userChar.ChainAttack(collision.collider.GetComponent<JucticeCoreBreakable>());
            }

            
        }
        else if(collision.gameObject.CompareTag("DoorButton"))
        {
            userChar.DestroyChainArm();
            collision.gameObject.GetComponent<DoorButton>().ButtonOn();
        }
        else if(collision.gameObject.CompareTag("ChainGrabable"))
        {
            userChar.ChainAttack(collision.collider.gameObject);
            collision.gameObject.GetComponent<PatrolBlock>().movable = true;
        }
        else
        {
            userChar.DestroyChainArm();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("HitBox"))
        {
            userChar.DestroyChainArm();
        }
    }



}
