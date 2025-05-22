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
    private ScoringSystem scoringSystem;
    private LivesSystem livesSystem;

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
            return;
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
        // Find the GameUI and ScoringSystem in the game scene
        if (scene.name == "GameScene")
        {
            gameUI = FindObjectOfType<GameUI>();
            scoringSystem = FindObjectOfType<ScoringSystem>();

            // Find or add LivesSystem to player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                livesSystem = player.GetComponent<LivesSystem>();
                if (livesSystem == null)
                {
                    livesSystem = player.AddComponent<LivesSystem>();
                    Debug.Log("Added LivesSystem to player");
                }
            }
            else
            {
                Debug.LogError("Player not found in scene!");
            }

            ResetGame();
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;

            // Get final score from scoring system if available
            if (scoringSystem != null)
            {
                currentScore = scoringSystem.GetCurrentScore();
                highScore = scoringSystem.GetHighScore();
            }

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

        // Reset lives system
        if (livesSystem != null)
        {
            livesSystem.ResetGame();
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

    public int GetCurrentScore()
    {
        return currentScore;
    }
} 