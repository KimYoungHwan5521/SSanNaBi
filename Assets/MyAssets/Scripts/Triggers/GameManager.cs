using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    GameObject stageTime;
    GameObject totalTime;
    GameObject stageTimeInst;
    GameObject totalTimeInst;
    TextMeshProUGUI stageTimeText;
    TextMeshProUGUI totalTimeText;

    private void Start()
    {
        stageTime = Resources.Load<GameObject>("Prefabs/UI/Stage Timer");
        totalTime = Resources.Load<GameObject>("Prefabs/UI/Total Timer");
    }

    private void Update()
    {
        시연용();
        LoadTimer();
        if (!stageClear && string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0)
        {
            stageClearTime += Time.unscaledDeltaTime;
        }
        if (string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0)
        {
            stageTimeText.text = $"{SceneManager.GetActiveScene().name} : {(int)stageClearTime / 60:00}\' {stageClearTime * 100 % 6000 / 100:00.00}\"";
            totalTimeText.text = $"Total : {(int)(totalClearTime + stageClearTime) / 60:00}\' {(totalClearTime + stageClearTime) * 100 % 6000 / 100:00.00}\"";
            
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
        if(mainCanvas == null)mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if (string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0) return;

        if(skipBtn == null)skipBtn = Resources.Load<GameObject>("Prefabs/UI/시연용 스킵버튼");
        if(skipBtnIsnt == null)
        {
            skipBtnIsnt = Instantiate(skipBtn, mainCanvas.transform);
            skipBtnIsnt.GetComponent<Button>().onClick.AddListener(Skip);
        }

    }

    void LoadTimer()
    {
        if (string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0)
        {
            if (stageTimeInst == null) stageTimeInst = Instantiate(stageTime, mainCanvas.transform);
            if(totalTimeInst == null) totalTimeInst = Instantiate(totalTime, mainCanvas.transform);
            if(stageTimeText == null) stageTimeText = stageTimeInst.GetComponent<TextMeshProUGUI>();
            if (totalTimeText == null) totalTimeText = totalTimeInst.GetComponent<TextMeshProUGUI>();

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
