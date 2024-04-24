using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] clips;
    public static AudioClip[] bgms;
    static AudioSource audioSource;
    static int playingBgmIndex;

    public void Awake()
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

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bgms = clips;
        PlayBgm(0);
    }

    public static void PlayBgm(int bgmIndex)
    {
        audioSource.Stop();
        audioSource.clip = bgms[bgmIndex];
        audioSource.Play();
        playingBgmIndex = bgmIndex;
    }

    public static int GetPlayingBgmIndex()
    {
        return playingBgmIndex;
    }
}
