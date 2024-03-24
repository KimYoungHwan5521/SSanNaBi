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

    public Transform[] nodeTransforms;
    public Vector3[] nodes;
    int nodeIndex = 0;
    bool isGoingBack = false;

    [SerializeField]Vector3 moveDirection;
    Vector3 departure;
    Vector3 destination;

    List<ContactInfo> upperCollisionList = new List<ContactInfo>();
    List<ContactInfo> removeCollisionList = new List<ContactInfo>();


    private void Start()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        startPoint.position = body.transform.parent.position;
        nodes[0] = startPoint.position;
        for (int i = 0; i < nodes.Length - 2; i++)
        {
            nodes[i + 1] = nodeTransforms[i].position;
        }
        nodes[nodes.Length - 1] = endPoint.position;

        departure = nodes[0];
        destination = nodes[1];
        body.transform.position = startPoint.position;

        lineRenderer.positionCount = nodes.Length;
        for (int i = 0; i < nodes.Length; i++)
        {
            lineRenderer.SetPosition(i, nodes[i]);
        }
    }

    private void FixedUpdate()
    {
        moveDirection = destination - departure;

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

        // �̵����� ����
        if((moveDirection * Time.fixedDeltaTime * moveSpeed / 10).magnitude > Vector2.Distance(destination, body.transform.position))
        {
            departure = destination;
            if(!isGoingBack)
            {
                if(nodeIndex > nodes.Length - 3)
                {
                    isGoingBack= true;
                    destination = nodes[nodeIndex];
                }
                else
                {
                    destination = nodes[nodeIndex + 2];
                    nodeIndex++;
                }
            }
            else
            {
                if(nodeIndex < 1)
                {
                    isGoingBack= false;
                    destination= nodes[nodeIndex + 1];
                }
                else
                {
                    destination = nodes[nodeIndex - 1];
                    nodeIndex--;
                }
            }

            
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
        for(int i=0; i<nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(nodes[i], nodes[i+1]);

        }
    }
}
