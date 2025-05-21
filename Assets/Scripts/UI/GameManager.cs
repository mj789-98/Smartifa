using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    private UIManager uiManager;

    [Header("Game State")]
    private bool isGameOver = false;
    private int currentScore = 0;
    private int coinsCollected = 0;
    private int highScore = 0;

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
        // Find the UIManager in the newly loaded scene
        uiManager = FindObjectOfType<UIManager>();
        
        // Reset game state if this is the game scene
        if (scene.name == "GameScene")
        {
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
            if (uiManager != null)
            {
                uiManager.ShowGameOver(currentScore, coinsCollected, highScore);
            }
        }
    }

    public void UpdateScore(int newScore)
    {
        if (!isGameOver)
        {
            currentScore = newScore;
            if (uiManager != null)
            {
                uiManager.UpdateScore(currentScore);
            }
        }
    }

    public void AddCoins(int amount)
    {
        if (!isGameOver)
        {
            coinsCollected += amount;
            if (uiManager != null)
            {
                uiManager.UpdateCoins(coinsCollected);
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
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateCoins(coinsCollected);
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