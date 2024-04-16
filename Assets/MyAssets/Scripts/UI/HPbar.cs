using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    public Image[] hps;

    Breakable player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Breakable>();
    }

    void Update()
    {
        if(player.HPCurrent > 20)
        {
            for (int i = 0; i < hps.Length; i++)
            {
                hps[i].color = Color.green;
            }
        }
        else if(player.HPCurrent > 10) 
        {
            for (int i = 0; i < hps.Length; i++)
            {
                hps[i].color = new Color(1, 0.75f, 0);
            }
        }
        else
        {
            for (int i = 0; i < hps.Length; i++)
            {
                hps[i].color = Color.red;
            }
        }
        
        for(int i=0; i<hps.Length; i++)
        {
            hps[hps.Length - i - 1].gameObject.SetActive(player.HPCurrent / 10 - i > 0);
        }

    }
}
