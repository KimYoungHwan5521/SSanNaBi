using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    [SerializeField]Breakable attacker;
    [SerializeField]int damage;
    bool isProjectile;

    public void Initialize(Breakable attaker, int damage, bool isProjectile = false)
    {
        this.attacker = attaker;
        this.damage = damage;
        this.isProjectile = isProjectile;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if(isProjectile && collision.gameObject.layer == 0) Destroy(gameObject);
        if(collision.gameObject.TryGetComponent(out Breakable victim))
        {
            if(attacker.CheckEnemy(victim))
            {
                if (!victim.CompareTag("Player") || victim.curInvincibleTime < 0)
                {
                    victim.TakeDamage(attacker, damage, collision.ClosestPoint(transform.position));
                    if (victim.TryGetComponent(out Character victimChar))
                    {
                        if (victimChar.anim != null)
                        { 
                            victimChar.anim.SetTrigger("doHit");
                            victimChar.isHit = true;
                        }
                    }
                    victim.curInvincibleTime = victim.invincibleTime;
                }
                if(isProjectile)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
