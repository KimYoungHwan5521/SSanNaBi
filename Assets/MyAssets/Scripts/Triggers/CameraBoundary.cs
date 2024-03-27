using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundary : MonoBehaviour
{
    [SerializeField]CinemachineConfiner2D confiner;
    [SerializeField]Collider2D thisCollider;

    void Start()
    {
        confiner = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineConfiner2D>();
        thisCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = thisCollider;

        }
    }

}
