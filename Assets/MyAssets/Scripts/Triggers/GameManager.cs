using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    private void Update()
    {
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
}
