using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public TextMeshProUGUI record;

    private void Start()
    {
        if (PlayerPrefs.GetFloat("TotalRecord") == 0) PlayerPrefs.SetFloat("TotalRecord", 5999.99f);
        if (PlayerPrefs.GetFloat("Stage1Record") == 0) PlayerPrefs.SetFloat("Stage1Record", 5999.99f);
        if (PlayerPrefs.GetFloat("Stage2Record") == 0) PlayerPrefs.SetFloat("Stage2Record", 5999.99f);
        if (PlayerPrefs.GetFloat("Stage3Record") == 0) PlayerPrefs.SetFloat("Stage3Record", 5999.99f);
        record.text = $"최고 기록 : {(int)PlayerPrefs.GetFloat("TotalRecord") / 60:00}\' {PlayerPrefs.GetFloat("TotalRecord") * 100 % 6000 / 100:00.00}\"";
    }

    public void GameStart()
    {
        SoundManager.PlayBgm(1);
        SceneManager.LoadScene("TutorialScene");
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying= false;
#else
        Application.Quit();
#endif
    }

    public void ResetRecord()
    {
        PlayerPrefs.DeleteAll();
        Start();
    }
}
