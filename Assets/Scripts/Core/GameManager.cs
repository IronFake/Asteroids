using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    
    public SpawnManager spawnManager;
    public GameObject player;

    [HideInInspector]
    public int playerScore = 0;
    public int maxLaserShots = 3;
    public float laserRecoveryTime = 10f;
    [HideInInspector]
    public int currentLaserShots;

    [HideInInspector]
    public bool isPlaying = false;
    [HideInInspector]
    public bool inGameOver = false;
    [HideInInspector]
    public bool inPause = false;
    [HideInInspector]
    public bool inMainMenu = true;
    [HideInInspector]
    public bool inSettings = false;

    public UIBehavior uIBehavior;

    public HighscoreTable highscoreTable;

    //private String playerName;

    void Awake()
    {
        //Singleton pattern
        if (main != null && main != this)
        {
            Destroy(gameObject);
            return;
        }
        main = this;

        currentLaserShots = maxLaserShots;   
    }

    private void Start()
    {
        inMainMenu = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inSettings)
            {
                uIBehavior.ExitFromSettings();
                return;
            }

            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!inPause && !inMainMenu && !inGameOver)
        {
            Time.timeScale = 0f;
            uIBehavior.Pause();
            String playerName = PlayerPrefs.GetString("name");
            highscoreTable.UpdateHighscoreTable(playerScore, playerName);
            highscoreTable.HighlightLine(false);
            isPlaying = false;
            inPause = true;
            return;
        }
        else
        {
            Time.timeScale = 1f;
            uIBehavior.Resume();
            isPlaying = true;
            inPause = false;
        }
    }

    public void DeleteAllObstaclesFromScene()
    {
        GameObject[] aliens = GameObject.FindGameObjectsWithTag("Alien");
        for (int i = 0; i < aliens.Length; i++)
        {
            Destroy(aliens[i]);
        }
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i]);
        }
    }

    IEnumerator LaserRecovery()
    {
        while (true)
        {
            yield return new WaitForSeconds(laserRecoveryTime);
            if (currentLaserShots < maxLaserShots)
            {
                currentLaserShots++;
                uIBehavior.UpdateLaserShoots(currentLaserShots);
            }
        }
    }

    public void StartGame()
    {
        inMainMenu = false;
        isPlaying = true;
        spawnManager.enabled = true;

        uIBehavior.UpdateLaserShoots(currentLaserShots);
        StartCoroutine(LaserRecovery());
    }

    public void RestartGame()
    {
        DeleteAllObstaclesFromScene();
        inGameOver = false;
        
        //Reset score
        playerScore = 0;
        UpdateScore(playerScore);

        //Reset laset shoots
        currentLaserShots = maxLaserShots;
        uIBehavior.UpdateLaserShoots(currentLaserShots);

        // Set player in start position
        player.transform.position = Vector2.zero;
        player.SetActive(true);

        spawnManager.enabled = true;
    }

    public void ResumeGame()
    {
        isPlaying = true;
        inPause = false;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        inGameOver = true;
        uIBehavior.GameOver();
        String playerName = PlayerPrefs.GetString("name");
        highscoreTable.AddHighscoreEntry(playerScore, playerName);
        highscoreTable.HighlightLine(true);

        player.SetActive(false);
        spawnManager.enabled = false;
    }

    public void UpdateScore(int points)
    {
        if (points == 0)
        {
            uIBehavior.UpdateScore(points);
        }
        else
        {
            playerScore += points; 
            uIBehavior.UpdateScore(playerScore);
        }
    }

    public void reduceTheNumberOfLaserShots()
    {
        currentLaserShots--;
        uIBehavior.UpdateLaserShoots(currentLaserShots);
    }
}
