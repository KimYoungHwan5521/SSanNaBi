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

    public float attackCoolTime;
    float curAttackCoolTime;

    public bool isAttack;


    void Start()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        curAttackCoolTime = attackCoolTime + Random.Range(-3f, 3f);
    }

    void Update()
    {
        if(marker == null)
        {
            marker = GetComponentInChildren<Breakable>().marker.GetComponentInChildren<Marker>();
            marker.isExecutor = true;
            marker.ExposeMarker();
        }
        
    }

    private void FixedUpdate()
    {
        if (!isAttack) 
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            if(curAttackCoolTime < 0)
            {
                isAttack= true;
                marker.HideMarker();
                anim.SetTrigger("doAttack");
                curAttackCoolTime = attackCoolTime + Random.Range(-3f, 3f);
            }
            curAttackCoolTime -= Time.deltaTime;

        }
    }


}
