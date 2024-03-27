using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIExecutor : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    GameObject player;
    public Marker marker;

    public Transform leftEdge;
    public Transform rightEdge;
    float leftLimit;
    float rightLimit;

    public float attackCoolTime;
    float curAttackCoolTime;

    public bool isActivate;
    public bool isAttack;
    public int attackCount = 0;


    void Start()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        leftLimit = leftEdge.position.x;
        rightLimit = rightEdge.position.x;
        curAttackCoolTime = attackCoolTime + Random.Range(-3f, 3f);
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isActivate) return;
        if (!isAttack) 
        {
            if(marker == null)
            {
                marker = GetComponentInChildren<Breakable>().marker.GetComponentInChildren<Marker>();
                marker.isExecutor = true;
                marker.ExposeMarker();
            }
            if(attackCount == -2) transform.position = new Vector3(rightLimit - 5f, transform.position.y, transform.position.z);
            else if(attackCount == -1)
            {
                transform.position = new Vector3(leftLimit + 5f, transform.position.y, transform.position.z);
            }
            else transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, leftLimit + 5f, rightLimit - 5f), transform.position.y, transform.position.z);
            if(curAttackCoolTime < 0)
            {
                isAttack= true;
                marker.HideMarker();
                attackCount++;
                if(attackCount == 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                anim.SetInteger("attackCount", attackCount);
                anim.SetTrigger("doAttack");
                curAttackCoolTime = attackCoolTime + Random.Range(-2f, 2f);
            }
            curAttackCoolTime -= Time.deltaTime;

        }
    }


}
