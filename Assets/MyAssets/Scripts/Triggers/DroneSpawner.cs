using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    GameObject droneSource;
    GameObject drone;

    void Start()
    {
        droneSource = Resources.Load<GameObject>("Prefabs/Characters/Drone");
    }

    void Update()
    {
        if (drone == null) drone = Instantiate(droneSource, transform.position, Quaternion.identity);
    }
}
