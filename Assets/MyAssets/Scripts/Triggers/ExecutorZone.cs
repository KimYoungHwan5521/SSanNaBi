using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ExecutorZone : MonoBehaviour
{
    Collider2D player;
    GameObject executor;

    ContactFilter2D filter;

    public float executorAttackCoolTime;
    float curExecutorAttackCoolTime;
    [SerializeField]bool isActivate;

    Vector2 spawnPosition;
    Vector2 discriminantVector;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        executor = Resources.Load<GameObject>("Prefabs/Characters/Executor(Field)");

        filter = new ContactFilter2D();
        filter.useTriggers= false;
        filter.SetLayerMask(LayerMask.GetMask("Default"));

        curExecutorAttackCoolTime = 0;
    }

    private void Update()
    {
        if (!isActivate) return;

        if(curExecutorAttackCoolTime < 0)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            Physics2D.CircleCast(player.bounds.center, 12.5f, Vector2.down, filter, hits);
            if(hits.Length > 0)
            {
                RaycastHit2D hit = hits[0];
                if (Mathf.Abs(hit.normal.x) < Mathf.Abs(hit.normal.y))
                {
                    // circleCast가 모서리를 캐스트하게 되는경우 노말값이 이상하게 나온다.
                    // 그래서 discriminantVector를 정의해해주었다.
                    discriminantVector = hit.normal * Vector2.up;
                    discriminantVector.Normalize();
                    float spawnPosX = Mathf.Clamp(hit.point.x, hit.collider.bounds.center.x - hit.collider.bounds.extents.x + 5.2f, hit.collider.bounds.center.x + hit.collider.bounds.extents.x - 5.2f);
                    spawnPosition = new Vector2(spawnPosX ,hit.collider.bounds.center.y + hit.collider.bounds.extents.y * discriminantVector.y) - discriminantVector * 13f;

                }
                else
                {
                    discriminantVector = hit.normal * Vector2.right;
                    discriminantVector.Normalize();
                    float spawnPosY = Mathf.Clamp(hit.point.y, hit.collider.bounds.center.y - hit.collider.bounds.extents.y + 5.2f, hit.collider.bounds.center.y + hit.collider.bounds.extents.y - 5.2f);
                    spawnPosition = new Vector2(hit.collider.bounds.center.x + hit.collider.bounds.extents.x * discriminantVector.x, spawnPosY) - discriminantVector * 13f;
                }
                GameObject inst = Instantiate(executor, spawnPosition, Quaternion.identity);

                if (discriminantVector == Vector2.down) inst.transform.Rotate(0, 0, 180f);
                else if (discriminantVector == Vector2.right) inst.transform.Rotate(0, 0, -90f);
                else if (discriminantVector == Vector2.left) inst.transform.Rotate(0, 0, 90f);


                curExecutorAttackCoolTime = executorAttackCoolTime;
            }

        }
        curExecutorAttackCoolTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isActivate= true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isActivate = false;
        }

    }

}
