using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameObject enemySource;
    GameObject enemy;

    public string enemyName;

    void Start()
    {
        enemySource = Resources.Load<GameObject>($"Prefabs/Characters/{enemyName}");
    }

    void Update()
    {
        if (enemy == null) enemy = Instantiate(enemySource, transform.position, Quaternion.identity);
    }
}
