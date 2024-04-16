using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Ally, Enemy, NPC, Object}

public abstract class Breakable : MonoBehaviour
{
    protected Rigidbody2D rigid;

    public GameObject marker;
    GameManager gameManager;

    public Team team;
    protected bool _isBreak;
    public virtual bool IsBreak
    {
        get { return _isBreak; }
        set 
        { 
            _isBreak = value;
            if(CompareTag("Player"))
            {
                gameManager.stageDeath++;
                gameManager.Load();
            }
            else if(CompareTag("Justice")) transform.parent.gameObject.AddComponent<DestroyTimer>().time =3f;
            else gameObject.AddComponent<DestroyTimer>().time = 3f;
            gameObject.layer = LayerMask.NameToLayer("Corpse");
            if(marker != null) Destroy(marker);
            if (flyable) rigid.gravityScale = 1f;
        }
    }

    [SerializeField]private int _hpCurrent;
    public int HPCurrent
    {
        get { return _hpCurrent; }
        set 
        { 
            _hpCurrent = Mathf.Clamp(value, 0, _hpMax); 
            if(_hpCurrent <= 0 && IsBreak == false) IsBreak = true;

        }
    }

    [SerializeField]private int _hpMax;
    public int HPCurrentMax
    {
        get { return _hpMax; }
        set { _hpMax = value; }
    }

    public int attackDamage;

    public bool flyable;

    public float invincibleTime;
    public float curInvincibleTime;

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (CompareTag("Player")) transform.position = gameManager.savePoint;
        if(team == Team.Enemy && (CompareTag("Enemy") || CompareTag("ExecutorBody")))
        {
            GameObject inst = Resources.Load<GameObject>("Prefabs/UI/EnemyMarker");
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            marker = Instantiate(inst, mainCanvas.transform);
            marker.AddComponent<Marker>().Initialize(gameObject);
        }
    }

    protected virtual void Update()
    {
        curInvincibleTime -= Time.deltaTime;
    }

    // 적이면(공격 가능하면) true
    public virtual bool CheckEnemy(Breakable target)
    {
        if(team == target.team)
        {
            return false;
        }
        else if(team == Team.Ally && !(target.team == Team.NPC) || team == Team.Enemy && target.team == Team.Ally)
        {
            return true;
        }
        return false;
    }

    public virtual int TakeDamage(Breakable from, int damage, Vector3 hitPoint)
    {
        HPCurrent -= damage;
        curInvincibleTime = invincibleTime;
        return damage;
    }

}
