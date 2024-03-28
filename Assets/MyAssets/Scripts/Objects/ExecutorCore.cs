using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutorCore : Breakable
{
    Animator anim;
    public AIExecutor executor;
    Marker markerScript;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        markerScript = marker.GetComponent<Marker>();
        markerScript.isExecutor= true;
    }

    protected override void Update()
    {
        if(markerScript!= null)
        {
            if(executor.isAttack && executor.attackCount >= 3) markerScript.ExposeMarker();
            else markerScript.HideMarker();
        }
        base.Update();

        if (executor.attackCount < 3) curInvincibleTime = invincibleTime;
    }

    public override int TakeDamage(Breakable from, int damage, Vector3 hitPoint)
    {
        anim.SetTrigger("doHit");
        if (HPCurrent == 300) executor.attackCount = -2;
        else if (HPCurrent == 200) executor.attackCount = -3;
        else if (HPCurrent == 100 ) executor.Break();
        return base.TakeDamage(from, damage, hitPoint);
    }
}
