using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] clips;
    public static AudioClip[] bgms;
    static AudioSource audioSource;
    static int playingBgmIndex;

    GameObject Volume;
    GameObject VolumeInst;

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
        Volume = Resources.Load<GameObject>("Prefabs/UI/Volume");

        audioSource = GetComponent<AudioSource>();
        bgms = clips;
        PlayBgm(0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(VolumeInst == null)
            {
                VolumeInst = Instantiate(Volume, GameObject.FindGameObjectWithTag("MainCanvas").transform);
                VolumeInst.GetComponentInChildren<Slider>().value = audioSource.volume;
            }
            else if(VolumeInst.activeSelf)
            {
                VolumeInst.SetActive(false);
            }
            else 
            {
                VolumeInst.SetActive(true); 
            }
        }
        if(VolumeInst!= null)
        {
            audioSource.volume = VolumeInst.GetComponentInChildren<Slider>().value;

        }
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
