using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutorActivater : MonoBehaviour
{
    AIExecutor executor;
    public Animator bossDoor;
    bool doorOpend;

    private void Start()
    {
        executor = GameObject.FindGameObjectWithTag("Executor").GetComponent<AIExecutor>();
        
    }

    private void Update()
    {
        if(executor.isBreak && !doorOpend)
        {
            Invoke("OpenDoor", 5f);
            doorOpend = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            executor.isActivate= true;

        }

    }

    void OpenDoor()
    {
        bossDoor.SetTrigger("Open");

    }
}
