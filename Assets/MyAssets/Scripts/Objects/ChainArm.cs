using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainArm : MonoBehaviour
{
    Vector2 grabForce = Vector2.zero;
    LineRenderer lineRenderer;
    public Transform user;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(grabForce != Vector2.zero)
        {
            GetComponent<Rigidbody2D>().AddForce(grabForce * 100);
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
    }

}
