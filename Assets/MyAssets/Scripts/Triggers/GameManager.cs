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

    [SerializeField]GameObject skipBtn;
    [SerializeField]GameObject mainCanvas;
    [SerializeField]GameObject skipBtnIsnt;
    
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
    public void 시연용()
    {
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
        Debug.Log(savePoint);
        if(savePoint != Vector2.zero)
        {
            if (string.Compare(SceneManager.GetActiveScene().name, "Stage1") == 0)
            {
                SceneManager.LoadScene("Stage2");
            }
            else if (string.Compare(SceneManager.GetActiveScene().name, "Stage2") == 0)
            {
                SceneManager.LoadScene("Stage3");

            }
            else if (string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0)
            {
                SceneManager.LoadScene("Title");
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
    }
}
