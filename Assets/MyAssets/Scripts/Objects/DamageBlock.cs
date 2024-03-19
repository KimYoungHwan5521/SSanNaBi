using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DamageBlock : Breakable
{
    void Start()
    {
        gameObject.AddComponent<DamageComponent>().Initialize(this, attackDamage);
    }

}
