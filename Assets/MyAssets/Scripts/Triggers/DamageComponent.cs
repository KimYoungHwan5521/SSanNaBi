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

    protected void OnTriggerStay2D(Collider2D collision)
    {
        //if(isProjectile && collision.gameObject.layer == 0 && !collision.CompareTag("CameraBoundary")) Destroy(gameObject);
        if(collision.gameObject.TryGetComponent(out Breakable victim))
        {
            if(attacker.CheckEnemy(victim))
            {
                if (!victim.CompareTag("Player"))
                {
                    victim.TakeDamage(attacker, damage, collision.ClosestPoint(transform.position));

                }
                else if(victim.curInvincibleTime < 0)
                {
                    if (victim.TryGetComponent(out Character victimChar) && !victimChar.isChainAttack)
                    {
                        victim.TakeDamage(attacker, damage, collision.ClosestPoint(transform.position));
                        victim.curInvincibleTime = victim.invincibleTime;
                        if (victimChar.anim != null)
                        { 
                            victimChar.anim.SetTrigger("doHit");
                            victimChar.isHit = true;
                        }

                    }

                }
                else
                {

                }
                if(isProjectile)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
