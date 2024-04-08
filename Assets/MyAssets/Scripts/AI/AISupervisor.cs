using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISupervisor : MonoBehaviour
{
    public SpriteRenderer eye;
    Transform playerT;
    Vector3 preferPosition;
    Vector3 preferDirection;

    public float moveSpeed;

    private void Start()
    {
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if(FindPlayer())
        {
            preferPosition = playerT.position;
            eye.material.color = new Color(1, Mathf.Clamp(eye.material.color.g - 3 * Time.fixedDeltaTime, 0, 1), 0);
        }
        else
        {
            eye.material.color = new Color(1, Mathf.Clamp(eye.material.color.g + Time.fixedDeltaTime, 0, 1), 0);

        }
        preferDirection = new Vector3(preferPosition.x - transform.position.x, preferPosition.y - transform.position.y, 0);

        transform.position += moveSpeed * Time.fixedDeltaTime * preferDirection;

    }

    bool FindPlayer()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        Physics2D.RaycastNonAlloc(playerT.position, transform.forward, hits, 2f, LayerMask.GetMask("Default"));
        for(int i=0; i<hits.Length; i++)
        {
            Debug.Log(hits[i].collider);
            if (hits[i].collider == null) return true;
            if (hits[i].collider.CompareTag("BackWall"))
            {
                Debug.Log(hits[i].collider.tag);
                return false;
            }
        }
        return true;
    }
}
