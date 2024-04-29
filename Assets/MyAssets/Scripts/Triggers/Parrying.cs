using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Parrying : MonoBehaviour
{
    public AIJustice parryinger;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player") || (collision.CompareTag("ChainArm") && !parryinger.justiceCore.isCoreActivated)) && !parryinger.controlledCharacter.isAttack && !parryinger.dashAttackReady && !parryinger.isWeak && !parryinger.beHit && !parryinger.nextPhase)
        {
            parryinger.controlledCharacter.isAttack = true;
            parryinger.controlledCharacter.anim.SetTrigger("doAttack");

        }
    }
}
