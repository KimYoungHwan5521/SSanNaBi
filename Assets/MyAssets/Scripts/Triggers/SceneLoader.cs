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
    [SerializeField]Image newRecordImage;

    bool showTotal;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (resultWindow != null)
        {
            resultTexts = resultWindow.GetComponentsInChildren<TextMeshProUGUI>();
            newRecordImage = resultWindow.GetComponentsInChildren<Image>()[2];
            newRecordImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0)
            {
                gameManager.stageClear = true;
                Time.timeScale = 0f;
                resultTexts[1].text = $"Stage Death : {gameManager.stageDeath}";
                resultTexts[2].text = $"Stage Clear Time : \n{(int)gameManager.stageClearTime / 60:00}\' {gameManager.stageClearTime * 100 % 6000 / 100:00.00} \"";
                if(PlayerPrefs.GetFloat($"{SceneManager.GetActiveScene().name}Record") > gameManager.stageClearTime)
                {
                    newRecordImage.gameObject.SetActive(true);
                    PlayerPrefs.SetFloat($"{SceneManager.GetActiveScene().name}Record", gameManager.stageClearTime);
                }
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
        if(string.Compare(SceneManager.GetActiveScene().name, "TutorialScene") != 0)
        {
            gameManager.totalDeath += gameManager.stageDeath;
            gameManager.totalClearTime+= gameManager.stageClearTime;
        }
        gameManager.stageDeath= 0;
        gameManager.stageClearTime= 0;
        if(string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0 && !showTotal)
        {
            resultTexts[0].text = "Game Clear!";
            resultTexts[1].text = $"Total Death : {gameManager.totalDeath}";
            resultTexts[2].text = $"Total Clear Time : \n{(int)gameManager.totalClearTime / 60 :00}\' {gameManager.totalClearTime * 100 % 6000 / 100:00.00}\"";
            if (PlayerPrefs.GetFloat("TotalRecord") > gameManager.totalClearTime)
            {
                newRecordImage.gameObject.SetActive(true);
                PlayerPrefs.SetFloat("TotalRecord", gameManager.totalClearTime);
            }
            showTotal = true;
            return;
        }
        Time.timeScale = 1f;
        gameManager.savePoint = Vector2.zero;
        gameManager.stageClear = false;
        if (string.Compare(SceneManager.GetActiveScene().name, "Stage1") == 0) SoundManager.PlayBgm(3);
        if (string.Compare(SceneManager.GetActiveScene().name, "Stage2") == 0) SoundManager.PlayBgm(5);
        if (string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0) SoundManager.PlayBgm(0);
        SceneManager.LoadScene(SceneName);
        if(string.Compare(SceneManager.GetActiveScene().name, "Stage3") == 0) Destroy(gameManager.gameObject);

    }
}
