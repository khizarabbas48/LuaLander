using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static int levelNumber = 1;
    private static int totalScore = 0;

    public static void ResetStaticData()
    {
        levelNumber = 1;
        totalScore = 0;
    }

    [SerializeField] private List<GameLevel> gameLevelsList;
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    private int score;
    private float time;
    private bool isTimerActive;

    private void Awake()
    {
        Instance = this; // Singleton pattern to ensure only one instance of GameManager exists
    }
    private void Start()
    {
        Lander.Instance.onCoinPickup += Lander_onCoinPickup; // Subscribe to the coin pickup event
        Lander.Instance.onLanded += Lander_onLanded; // Subscribe to the landed event
        Lander.Instance.OnStateChange += Lander_OnStateChange; // Subscribe to the state change event

        GameInput.Instance.OnMenuButtonPressed += GameInput_OnMenuButtonPressed;
        LoadCurrentLevel();
    }

    private void Update()
    {
        if (isTimerActive)
        {
            time += Time.deltaTime; // Increment time by the time elapsed since the last frame
        }
    }

    private void GameInput_OnMenuButtonPressed(object sender, EventArgs e)
    {
        PauseUnpauseGame();
    }

    // Method to load the current level based on levelNumber
    private void LoadCurrentLevel()
    {
        GameLevel gameLevel = GetGameLevel();
        GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
        Lander.Instance.transform.position = spawnedGameLevel.GetLanderStartPosition();

        // Set the camera to follow the lander initially
        cinemachineCamera.Target.TrackingTarget = spawnedGameLevel.GetCameraStartTargetTransform();
        CinemachineCameraZoom2D.Instance.SetTargetOrthographicSize(spawnedGameLevel.GetZoomedOutOrthographicSize());

    }

    private GameLevel GetGameLevel()
    {
        foreach (GameLevel gameLevel in gameLevelsList)
        {
            if (gameLevel.GetLevelNumber() == levelNumber)
            {
                return gameLevel;
            }
        }
        return null;
    }

    // Event handler for when the lander's state changes
    private void Lander_OnStateChange(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == Lander.State.Normal;
        if (e.state == Lander.State.Normal)
        {
            cinemachineCamera.Target.TrackingTarget = Lander.Instance.transform; // Follow the lander
            CinemachineCameraZoom2D.Instance.SetToNormalOrthographicSize(); // Zoom in to normal size
        }
    }

    // Event handler for when the lander lands
    private void Lander_onLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    // Event handler for when the lander picks up a coin
    private void Lander_onCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500);
    }

    // Method to add score and log the current score
    public void AddScore(int addScoreAmout)
    {
        score += addScoreAmout;
        Debug.Log(score);
    }

    public int GetScore()
    {
        return score;
    }
    public float GetTime()
    {
        return time;
    }

    public int GetTotalScore()
    {
        return totalScore + score;
    }

    // Methods to navigate between levels
    public void GotoNextLevel()
    {
        levelNumber++;
        totalScore += score;
        if(GetGameLevel() == null)
        {
            // No more levels, go to game over scene
            levelNumber--; // Revert level number increment
            totalScore -= score; // Revert score addition
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
        }
        else
        {
            // We have more levels
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        }
    }

    // Method to retry the current level
    public void RetryLevel()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    // Method to get the current level number
    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause game time
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume game time
        OnGameUnpaused?.Invoke(this, EventArgs.Empty);
    }
    public void PauseUnpauseGame()
    {
        if (Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

}
