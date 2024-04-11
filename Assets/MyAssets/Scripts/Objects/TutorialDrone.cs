using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrone : MonoBehaviour
{
    LineRenderer lineRenderer;
    Character target;

    private void Start()
    {
        lineRenderer= GetComponentInChildren<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            target = collision.GetComponent<Character>();
            Invoke("Attack", 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            target = null;
        }

    }

    void Attack()
    {
        if(target != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(1, target.transform.position);
            target.TakeDamage(GetComponent<Character>(), 1, target.transform.position);
            Invoke("AttackEnd", 0.1f);
        }
    }

    void AttackEnd()
    {
        lineRenderer.enabled = false;
    }
}
