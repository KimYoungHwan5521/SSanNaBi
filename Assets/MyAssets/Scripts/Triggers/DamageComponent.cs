using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    Breakable attacker;
    int damage;

    public void Initialize(Breakable attaker, int damage)
    {
        this.attacker = attaker;
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Breakable victim))
        {
            if(attacker.CheckEnemy(victim))
            {
                victim.TakeDamage(attacker, damage);
                if(victim.TryGetComponent(out Character victimChar))
                {
                    victimChar.anim.SetTrigger("doHit");
                }
            }
        }
    }
}
