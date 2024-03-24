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
                rigid.velocity = Vector2.down * moveSpeed * Time.fixedDeltaTime;
            }
            else if(transform.position.y < initialPosition.y)
            {
                rigid.velocity = Vector2.up * moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                rigid.velocity= Vector2.zero;
            }
            //Debug.Log(rigid.velocity);
            updateTime= Time.time;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("ChainArm") || collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
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
        if (collision.collider.CompareTag("ChainArm") || collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
        {
            if (collision.collider.TryGetComponent(out Character character))
            {
                character.velocityCorrection = Vector2.zero;
            }
            collisioinCount--;
        }

    }
}
