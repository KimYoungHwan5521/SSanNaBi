using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;
    GameManager gameManager;
    public GameObject resultWindow;
    TextMeshProUGUI[] resultTexts;

    bool showTotal;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if(resultWindow != null) resultTexts = resultWindow.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().buildIndex != 0)
            {
                gameManager.stageClear = true;
                Time.timeScale = 0f;
                resultTexts[1].text = $"Stage Death : {gameManager.stageDeath}";
                resultTexts[2].text = $"Stage Clear Time : \n{(int)gameManager.stageClearTime / 60:00} : {gameManager.stageClearTime * 100 % 6000 / 100:00.00}";
                resultWindow.SetActive(true);
            }
            else
            {
                NextScene();
            }
        }
    }


    public void NextScene()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameManager.totalDeath += gameManager.stageDeath;
            gameManager.totalClearTime+= gameManager.stageClearTime;
        }
        gameManager.stageDeath= 0;
        gameManager.stageClearTime= 0;
        if(SceneManager.GetActiveScene().buildIndex == 3 && !showTotal)
        {
            resultTexts[0].text = "Game Clear!";
            resultTexts[1].text = $"Total Death : {gameManager.totalDeath}";
            resultTexts[2].text = $"Total Clear Time : \n{(int)gameManager.totalClearTime / 60 :00} : {gameManager.totalClearTime * 100 % 6000 / 100:00.00}";
            showTotal = true;
            return;
        }
        Time.timeScale = 1f;
        gameManager.savePoint = Vector2.zero;
        gameManager.stageClear = false;
        SceneManager.LoadScene(SceneName);

    }
}
