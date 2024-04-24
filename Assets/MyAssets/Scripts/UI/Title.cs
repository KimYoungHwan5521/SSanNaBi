using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void GameStart()
    {
        //SceneManager.LoadScene("TutorialScene");
        SceneManager.LoadScene("Stage1");
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying= false;
#else
        Application.Quit();
#endif
    }
}
