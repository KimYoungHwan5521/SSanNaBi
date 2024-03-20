using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBlock : MonoBehaviour
{
    public Transform body;
    public Transform startPoint;
    public Transform endPoint;

    Rigidbody2D rigid;
    LineRenderer lineRenderer;

    public float moveSpeed;

    Vector3 moveDirection;
    Vector3 destination;

    List<ContactInfo> upperCollisionList = new List<ContactInfo>();
    List<ContactInfo> removeCollisionList = new List<ContactInfo>();

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        body.transform.position = startPoint.transform.position;
        destination = endPoint.position;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }

    private void FixedUpdate()
    {

        if (destination == endPoint.position)
        {
            moveDirection = endPoint.position - startPoint.position;
        }
        else
        {
            moveDirection = startPoint.position - endPoint.position;
        }
        moveDirection.Normalize();
        rigid.velocity = moveDirection * moveSpeed * Time.fixedDeltaTime;

        // �ڿ���Ÿ���� ���� ����Ʈ�� ����
        removeCollisionList = upperCollisionList.FindAll(target => target.time < Time.time - Character.coyoteTime);
        foreach(ContactInfo c in removeCollisionList)
        {
            if(c.other != null && c.other.TryGetComponent(out Character removeC))
            {
                removeC.velocityCorrection = Vector2.zero;
            }
            upperCollisionList.Remove(c);
        }
        // �������� ������Ʈ�� ���� �����̰�
        // �������� ������Ʈ���� �ӵ��� �ְ� ������ �Z���� ���� �����ϰ� ����ӵ��� �ٽ� �־��ش�.
        for(int i=0; i<upperCollisionList.Count;i++)
        {
            if(upperCollisionList[i].other != null && upperCollisionList[i].other.TryGetComponent(out Character character))
            {
                character.velocityCorrection = rigid.velocity * Vector2.right;
            }

        }
        

        if((moveDirection * Time.fixedDeltaTime).magnitude > Vector2.Distance(destination, body.transform.position))
        {
            if(destination == endPoint.position) destination = startPoint.position;
            else destination = endPoint.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        // ������Ʈ�� ���ʿ��� ����ִ� ���������� ����
        ContactPoint2D myContact = System.Array.Find(contacts, target => target.otherCollider.gameObject == gameObject && Vector2.Angle(target.normal, Vector2.up) < 46f);

        // �ߺ�üũ
        int collisionIndex = upperCollisionList.FindIndex(target => target.other == collision.gameObject);
        if (collisionIndex == -1)
        {
            // �Ȱ��� ���� ���ٸ� ����Ʈ�� �߰�
            upperCollisionList.Add(new ContactInfo(collision.gameObject, myContact, Time.time));
        }
        else
        {
            // �̹� ��� �ִٸ� Ÿ�Ӹ� ����
            upperCollisionList[collisionIndex] = new ContactInfo(collision.gameObject, myContact, Time.time);
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
}
