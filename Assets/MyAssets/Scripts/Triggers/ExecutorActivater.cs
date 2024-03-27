using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutorActivater : MonoBehaviour
{
    AIExecutor executor;

    private void Start()
    {
        executor = GameObject.FindGameObjectWithTag("Executor").GetComponent<AIExecutor>();
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            executor.isActivate= true;

        }

    }
}
