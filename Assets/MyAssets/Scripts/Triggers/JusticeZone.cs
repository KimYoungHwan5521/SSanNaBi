using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JusticeZone : MonoBehaviour
{
    GameObject justiceInst;
    GameObject justiceAlterEgoInst;
    public Transform spawnPosition;
    public Animator entryDoor;
    public Animator exitDoor;

    [SerializeField]GameObject justiceMain;
    [SerializeField]GameObject justiceAlterEgo;
    bool firstJusticeAppeared;
    bool secondJusticeAppeard;
    [SerializeField]bool justiceDefeated;

    public bool justiceMainDead;
    public bool justiceAlterEgoDead;

    public float alterEgoSpawnCoolTime;
    float curAlterEgoSpawnCoolTime;

    private void Start()
    {
        justiceInst = Resources.Load<GameObject>("Prefabs/Characters/Justice");
        justiceAlterEgoInst = Resources.Load<GameObject>("Prefabs/Characters/JusticeAlterEgo");

        justiceAlterEgoDead = true;
        curAlterEgoSpawnCoolTime = 6f;
        entryDoor.SetBool("isOpen", true);
    }

    private void Update()
    {
        if(justiceDefeated) 
        {
            if (!exitDoor.GetBool("isOpen")) exitDoor.SetBool("isOpen", true);
            return; 
        }
        if(!secondJusticeAppeard && firstJusticeAppeared && justiceMainDead)
        {
            justiceMainDead = false;
            Invoke(nameof(SpawnSecondJustice), 3f);
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
                else if(!justiceMain.GetComponentInChildren<AIJustice>().isWeak)
                {
                    curAlterEgoSpawnCoolTime -= Time.deltaTime;
                }
            }
            if(justiceMainDead)
            {
                if (justiceAlterEgo == null) return;
                if(!justiceAlterEgoDead) justiceAlterEgo.GetComponentInChildren<Character>().TakeDamage(null, 1000, justiceAlterEgo.transform.position);
                justiceDefeated= true;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (firstJusticeAppeared) { return; }
        if(collision.CompareTag("Player") && justiceMain == null)
        {
            if (SoundManager.GetPlayingBgmIndex() != 4) SoundManager.PlayBgm(4);
            justiceMain = Instantiate(justiceInst, spawnPosition.position, Quaternion.identity);
            justiceMain.GetComponentInChildren<AIJustice>().justiceZone = this;
            firstJusticeAppeared= true;
            entryDoor.SetBool("isOpen", false);
        }
    }

    void SpawnSecondJustice()
    {
        justiceMain = Instantiate(justiceInst, spawnPosition.position, Quaternion.identity);
        justiceMain.GetComponentInChildren<AIJustice>().justiceZone = this;
        secondJusticeAppeard = true;
    }
}
