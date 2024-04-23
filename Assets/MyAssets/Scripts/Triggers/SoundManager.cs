using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] bgms;

    public void Awake()
    {
        if(instance == null)
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void PlayBgm(int bgmIndex, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(bgms[bgmIndex], position);
    }
}
