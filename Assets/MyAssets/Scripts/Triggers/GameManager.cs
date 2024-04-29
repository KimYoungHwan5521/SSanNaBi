using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Vector2 savePoint;
    public int stageDeath = 0;
    public float stageClearTime = 0;
    public int totalDeath = 0;
    public float totalClearTime = 0;
    public bool stageClear;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    GameObject skipBtn;
    GameObject mainCanvas;
    GameObject skipBtnIsnt;
    
    private void Update()
    {
        시연용();
        if (!stageClear)
        {
            stageClearTime += Time.unscaledDeltaTime;
        }
    }

    public void Load()
    {
        Time.timeScale = 1f;
        Invoke(nameof(ReloadScene), 3f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 시연용
    // -> 시연용에서 튜토리얼 스킵용으로
    public void 시연용()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0) return;

        if(skipBtn == null)skipBtn = Resources.Load<GameObject>("Prefabs/UI/시연용 스킵버튼");
        if(mainCanvas == null)mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if(skipBtnIsnt == null)
        {
            skipBtnIsnt = Instantiate(skipBtn, mainCanvas.transform);
            skipBtnIsnt.GetComponent<Button>().onClick.AddListener(Skip);
        }

    }

    public void Skip()
    {

        SceneManager.LoadScene("Stage1");
        
        /*
        if (savePoint != Vector2.zero)
        {
            if (string.Compare(SceneManager.GetActiveScene().name, "Stage1") == 0)
            {
                SceneManager.LoadScene("Stage2");
                SoundManager.PlayBgm(3);
            }
            else if (string.Compare(SceneManager.GetActiveScene().name, "Stage2") == 0)
            {
                SceneManager.LoadScene("Stage3");
                SoundManager.PlayBgm(5);

            }
            else if (string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0)
            {
                SceneManager.LoadScene("Title");
                SoundManager.PlayBgm(0);
                Destroy(gameObject);
            }
            savePoint = new Vector2(0, 0);
        }
        else
        {
            if(string.Compare(SceneManager.GetActiveScene().name, "Stage1") == 0)
            {
                savePoint = new Vector2(475.8f, -115.6f);
            }
            else if(string.Compare(SceneManager.GetActiveScene().name, "Stage2") == 0)
            {
                savePoint = new Vector2(320.8f, -151.8f);

            }
            else if(string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0)
            {
                savePoint = new Vector2(984.6f, -143.4f);

            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(skipBtnIsnt);
        }
        */
    }
}
