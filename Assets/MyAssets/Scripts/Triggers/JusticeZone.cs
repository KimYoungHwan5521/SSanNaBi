using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JusticeZone : MonoBehaviour
{
    GameObject justiceInst;
    GameObject justiceAlterEgoInst;
    public Transform spawnPosition;

    GameObject justiceMain;
    GameObject justiceAlterEgo;
    [SerializeField]bool firstJusticeAppeared;
    [SerializeField] bool secondJusticeAppeard;
    [SerializeField] bool justiceDefeated;

    public bool justiceMainDead;
    public bool justiceAlterEgoDead;

    public float alterEgoSpawnCoolTime;
    [SerializeField]float curAlterEgoSpawnCoolTime;

    private void Start()
    {
        justiceInst = Resources.Load<GameObject>("Prefabs/Characters/Justice");
        justiceAlterEgoInst = Resources.Load<GameObject>("Prefabs/Characters/JusticeAlterEgo");

        justiceAlterEgoDead = true;
        curAlterEgoSpawnCoolTime = 6f;
}

    private void Update()
    {
        if(justiceDefeated) { return; }
        if(!secondJusticeAppeard && firstJusticeAppeared && justiceMainDead)
        {
            justiceMainDead = false;
            Invoke("SpawnSecondJustice", 3f);
            secondJusticeAppeard = true;
        }
        if(secondJusticeAppeard)
        {
            if (!justiceMainDead && justiceAlterEgoDead)
            {
                if(curAlterEgoSpawnCoolTime < 0)
                {
                    justiceAlterEgoDead = false;
                    justiceAlterEgo = Instantiate(justiceAlterEgoInst, justiceMain.transform.position, justiceMain.transform.rotation);
                    justiceAlterEgo.GetComponentInChildren<AIJustice>().justiceZone = this;
                    curAlterEgoSpawnCoolTime = alterEgoSpawnCoolTime;
                }
                else
                {
                    curAlterEgoSpawnCoolTime -= Time.deltaTime;
                }
            }
            if(justiceMainDead)
            {
                if(!justiceAlterEgoDead) justiceAlterEgo.GetComponent<Character>().TakeDamage(null, 1000, justiceAlterEgo.transform.position);
                justiceDefeated= true;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (firstJusticeAppeared) { return; }
        if(collision.CompareTag("Player") && justiceMain == null)
        {
            justiceMain = Instantiate(justiceInst, spawnPosition.position, Quaternion.identity);
            justiceMain.GetComponentInChildren<AIJustice>().justiceZone = this;
            firstJusticeAppeared= true;
        }
    }

    void SpawnSecondJustice()
    {
        justiceMain = Instantiate(justiceInst, spawnPosition.position, Quaternion.identity);
        justiceMain.GetComponentInChildren<AIJustice>().justiceZone = this;
    }
}
