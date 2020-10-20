using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour
{
    [Header("UI")]
    public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;


    public GameObject settingMenuUI;
    public Slider misicSlider;
    public Slider SFXSlider;
    
    public GameObject inGameUI;
#if MOBILE_INPUT
    public GameObject mobileControlUI;
#endif

    private Text scoreText;
    private Text laserShootsText;

    public GameObject tableUI;

    public GameObject[] backgrounds;
    public int defaultBackground = 0;

    
    private List<Toggle> toggleList;

    
    private GameObject currentBackground;

    private GameManager gameManager;


    private void Start()
    {
        gameManager = GameManager.main;

        // Set saved background
        defaultBackground = PlayerPrefs.GetInt("backgroundIndex");
        CreateBackgroundObject(defaultBackground);

        scoreText = inGameUI.transform.Find("ScoreText").GetComponent<Text>();
        laserShootsText = inGameUI.transform.Find("LaserShoots").Find("Text").GetComponent<Text>();
    }

    public void Play()
    {
#if MOBILE_INPUT
        mobileControlUI.SetActive(true);
#endif
        tableUI.SetActive(false);
        mainMenuUI.SetActive(false);
        inGameUI.SetActive(true);
        gameManager.StartGame();
    }

    public void Restart()
    {
#if MOBILE_INPUT
        mobileControlUI.SetActive(true);
#endif
        tableUI.SetActive(false);
        gameOverUI.SetActive(false);
        inGameUI.SetActive(true);
        gameManager.RestartGame();
    }

    public void Pause()
    {
#if MOBILE_INPUT
        mobileControlUI.SetActive(false);
#endif
        inGameUI.SetActive(false);
        tableUI.SetActive(true);
        pauseMenuUI.SetActive(true);
    }

    public void Resume()
    {
#if MOBILE_INPUT
        mobileControlUI.SetActive(true);
#endif
        tableUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        inGameUI.SetActive(true);
        gameManager.ResumeGame();
    }

    public void GameOver()
    {
#if MOBILE_INPUT
        mobileControlUI.SetActive(false);
#endif
        inGameUI.SetActive(false);
        tableUI.SetActive(true);
        gameOverUI.SetActive(true);
    }

    public void Settings()
    {
        GameManager.main.inSettings = true;
        tableUI.SetActive(false);
        if (GameManager.main.inGameOver)
        {
            gameOverUI.SetActive(false);
        }
        else if (GameManager.main.inPause)
        {
            pauseMenuUI.SetActive(false);
        }
        else
        {
            mainMenuUI.SetActive(false);
        }
        settingMenuUI.SetActive(true);

        // Select toggle 
        toggleList = new List<Toggle>(GetComponentsInChildren<Toggle>());
        toggleList[defaultBackground].isOn = true;

        //Set volume in sliders
        misicSlider.value = PlayerPrefs.GetFloat("musicVol");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVol");
    }

    public void ExitFromSettings()
    {
        GameManager.main.inSettings = false;
        settingMenuUI.SetActive(false);
        tableUI.SetActive(true);
        if (GameManager.main.inGameOver)
        {
            gameOverUI.SetActive(true);
        }
        else if (GameManager.main.inPause)
        {
            pauseMenuUI.SetActive(true);
        }
        else
        {
            mainMenuUI.SetActive(true);
        }
        
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OnClick(Toggle sender)
    {
        if (sender.isOn)
        {
            int index = toggleList.IndexOf(sender);
            CreateBackgroundObject(index);
        }  
    }

    private void CreateBackgroundObject(int index)
    {
        Destroy(currentBackground);
        switch (index)
        {
            case 0:
                currentBackground = Instantiate(backgrounds[0]);
                break;
            case 1:
                currentBackground = Instantiate(backgrounds[1]);
                break;
            case 2:
                currentBackground = Instantiate(backgrounds[2]);
                break;
        }

        defaultBackground = index;
        PlayerPrefs.SetInt("backgroundIndex", defaultBackground);
    }

    public void UpdateLaserShoots(int value)
    {
        laserShootsText.text = "= " + value;
    }

    public void UpdateScore(int score)
    {        
        scoreText.text = score.ToString();
    }

}
