using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainArm : MonoBehaviour
{
    Vector2 grabForce = Vector2.zero;
    LineRenderer lineRenderer;
    public Transform user;
    public bool isChainArmGrab = false;

    List<GameObject> chainsList = new List<GameObject>();
    GameObject chain;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        chain = Resources.Load<GameObject>("Prefabs/Character/Chain");
    }

    private void FixedUpdate()
    {
        if(grabForce != Vector2.zero)
        {
            isChainArmGrab= true;
            GetComponent<Rigidbody2D>().AddForce(grabForce * 1000);
        }
        /*
        if(!isChainArmGrab)
        {
            if(Vector2.Distance(user.position, transform.position) > chainsList.Count + 1)
            {
                Vector3 chainSpawnPosition = transform.position - user.position;
                chainSpawnPosition *= (float)(chainsList.Count + 1) / (chainsList.Count + 2);
                chainSpawnPosition += user.position;
                GameObject inst = Instantiate(chain, chainSpawnPosition, transform.rotation);
                HingeJoint2D instHJ = inst.GetComponent<HingeJoint2D>();
                //instDJ.enabled = true;
                if(chainsList.Count == 0) instHJ.connectedBody = GetComponent<Rigidbody2D>();
                else instHJ.connectedBody = chainsList[chainsList.Count - 1].GetComponent<Rigidbody2D>();
                HingeJoint2D userHJ = user.GetComponent<HingeJoint2D>();
                userHJ.enabled = true;
                userHJ.connectedBody = inst.GetComponent<Rigidbody2D>();
                chainsList.Add(inst);
            }
        }
        */
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
