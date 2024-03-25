using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlock : Breakable
{
    protected override void Start()
    {
        gameObject.AddComponent<DamageComponent>().Initialize(this, attackDamage);
    }

}
