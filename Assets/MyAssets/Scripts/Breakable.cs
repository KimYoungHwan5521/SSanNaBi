using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Ally, Enemy, NPC, Object}

public abstract class Breakable : MonoBehaviour
{
    public Team team;
    private bool _isBreak;
    public bool IsBreak
    {
        get { return _isBreak; }
        set { _isBreak = value; }
    }

    [SerializeField]private int _hpCurrent;
    public int HPCurrent
    {
        get { return _hpCurrent; }
        set { _hpCurrent = Mathf.Clamp(value, 0, _hpMax); }
    }

    [SerializeField]private int _hpMax;
    public int HPCurrentMax
    {
        get { return _hpMax; }
        set { _hpMax = value; }
    }

    // ���̸�(���� �����ϸ�) true
    public virtual bool CheckEnemy(Breakable target)
    {
        if(team == Team.Ally && !(target.team == Team.NPC) || team == Team.Enemy && target.team == Team.Ally)
        {
            return true;
        }
        return false;
    }
}
