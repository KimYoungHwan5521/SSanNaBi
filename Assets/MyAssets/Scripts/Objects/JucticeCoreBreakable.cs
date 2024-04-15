using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JucticeCoreBreakable : Breakable
{
    public GameObject justice;

    public override bool IsBreak 
    { 
        get => base.IsBreak; 
        set
        {
            _isBreak = value;
            gameObject.layer = LayerMask.NameToLayer("Corpse");
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Animator>().SetTrigger("doDeath");
            transform.parent.GetComponentInParent<JusticeCore>().isCoreActivated = false;
            justice.GetComponent<AIJustice>().JusticeCoreHit();
        }
    }

    protected override void Start()
    {

    }
}
