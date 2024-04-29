using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownBlock : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector2 initialPosition;
    [SerializeField]int collisioinCount;

    float updateTime;

    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        initialPosition= transform.position;
        collisioinCount = 0;
        updateTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time > updateTime + Character.coyoteTime)
        {
            if(collisioinCount > 0)
            {
                rigid.velocity = moveSpeed * Time.fixedDeltaTime * Vector2.down;
            }
            else if(transform.position.y < initialPosition.y)
            {
                rigid.velocity = moveSpeed * Time.fixedDeltaTime * Vector2.up;
            }
            else
            {
                rigid.velocity= Vector2.zero;
            }
            updateTime= Time.time;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
        {
            if(collision.collider.TryGetComponent(out Character character))
            {
                character.velocityCorrection = Vector2.down * 10;
            }
            collisioinCount++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
        {
            if (collision.collider.TryGetComponent(out Character character))
            {
                character.velocityCorrection = Vector2.zero;
            }
            collisioinCount--;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("gd");
        if(collision.CompareTag("ChainArm"))
        {
            Debug.Log("gd2");
            collisioinCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ChainArm"))
        {
            collisioinCount--;
        }

    }
}
