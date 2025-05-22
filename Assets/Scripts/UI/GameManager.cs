using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    private bool isGameOver = false;
    private int currentScore = 0;
    private int coinsCollected = 0;
    private int highScore = 0;

    // Reference to the current scene's UI
    private GameUI gameUI;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the GameUI in the game scene
        if (scene.name == "GameScene")
        {
            gameUI = FindObjectOfType<GameUI>();
            ResetGame();
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            // Check for new high score
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

            // Show game over UI
            if (gameUI != null)
            {
                gameUI.ShowGameOver(currentScore, coinsCollected, highScore);
            }
        }
    }

    public void UpdateScore(int newScore)
    {
        if (!isGameOver)
        {
            currentScore = newScore;
            if (gameUI != null)
            {
                gameUI.UpdateScore(currentScore);
            }
        }
    }

    public void AddCoins(int amount)
    {
        if (!isGameOver)
        {
            coinsCollected += amount;
            if (gameUI != null)
            {
                gameUI.UpdateCoins(coinsCollected);
            }
        }
    }

    public void ResetGame()
    {
        isGameOver = false;
        currentScore = 0;
        coinsCollected = 0;
        Time.timeScale = 1f;
        
        // Update UI with reset values
        if (gameUI != null)
        {
            gameUI.UpdateScore(currentScore);
            gameUI.UpdateCoins(coinsCollected);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public int GetHighScore()
    {
        return highScore;
    }
} 