using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIExecutor : MonoBehaviour
{
    Animator anim;
    ExecutorCore core;

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

    public bool isTornado;
    public bool isBreak;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        core= GetComponentInChildren<ExecutorCore>();

        player = GameObject.FindGameObjectWithTag("Player");

        leftLimit = leftEdge.position.x;
        rightLimit = rightEdge.position.x;
        curAttackCoolTime = attackCoolTime + Random.Range(-3f, 3f);
    }

    private void FixedUpdate()
    {
        if (!isActivate || isBreak) return;
        if (!isAttack) 
        {
            if(marker == null)
            {
                marker = core.marker.GetComponentInChildren<Marker>();
                marker.isExecutor = true;
                marker.ExposeMarker();
            }
            if(attackCount == -2) transform.position = new Vector3(rightLimit - 5f, transform.position.y, transform.position.z);
            else if(attackCount == -1)
            { 
                transform.position = new Vector3(leftLimit + 5f, transform.position.y, transform.position.z);
            }
            else
            {
                float velocity = Mathf.Clamp(player.transform.position.x, transform.position.x - Time.fixedDeltaTime * 30, transform.position.x + Time.fixedDeltaTime * 30);
                transform.position = new Vector3(Mathf.Clamp(velocity, leftLimit + 5f, rightLimit - 5f), transform.position.y, transform.position.z);
            }
            if(curAttackCoolTime < 0)
            {
                isAttack= true;
                marker.HideMarker();
                attackCount++;
                if (attackCount == 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                else if (attackCount == -2) isTornado = true;
                else if (attackCount == 3) core.curInvincibleTime = 0;
                anim.SetInteger("attackCount", attackCount);
                anim.SetTrigger("doAttack");
                curAttackCoolTime = attackCoolTime + Random.Range(-2f, 2f);
            }
            curAttackCoolTime -= Time.deltaTime;

        }
        else if(isTornado)
        {
            float velocity = Mathf.Clamp(player.transform.position.x, transform.position.x - Time.fixedDeltaTime * 5, transform.position.x + Time.fixedDeltaTime * 5);
            transform.position = new Vector3(Mathf.Clamp(velocity, leftLimit + 5f, rightLimit - 5f), transform.position.y, transform.position.z);

        }
    }

    public void Break()
    {
        isBreak= true;
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        for (int i = 0;i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }
        gameObject.AddComponent<DestroyTimer>().time = 11f;
        anim.SetTrigger("doDeath");
    }


}
