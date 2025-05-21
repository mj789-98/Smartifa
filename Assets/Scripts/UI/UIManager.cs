using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button startGameButton;
    public Button settingsButton;
    public Button leaderboardButton;
    public Button exitButton;
    public Button characterSelectionButton;
    public Button storeButton;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI coinsCollectedText;
    public TextMeshProUGUI highScoreText;
    public Button retryButton;
    public Button mainMenuButton;

    [Header("In-Game HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;

    private void Start()
    {
        // Initialize Main Menu buttons
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        if (characterSelectionButton != null)
            characterSelectionButton.onClick.AddListener(OpenCharacterSelection);
        if (storeButton != null)
            storeButton.onClick.AddListener(OpenStore);

        // Initialize Game Over buttons
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Hide Game Over panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Initialize HUD with zero values
        UpdateScore(0);
        UpdateCoins(0);
    }

    public void ShowGameOver(int finalScore, int coinsCollected, int highScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
                finalScoreText.text = "Final Score: " + finalScore;
            
            if (coinsCollectedText != null)
                coinsCollectedText.text = "Coins: " + coinsCollected;
            
            if (highScoreText != null)
                highScoreText.text = "High Score: " + highScore;

            Time.timeScale = 0f; // Pause the game
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void UpdateCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coins;
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // Ensure normal time scale
        SceneManager.LoadScene("GameScene"); // Make sure to set up this scene name in build settings
    }

    private void RetryGame()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure to set up this scene name in build settings
    }

    private void OpenSettings()
    {
        // Placeholder for settings functionality
        Debug.Log("Settings button clicked - Functionality to be implemented");
    }

    private void OpenLeaderboard()
    {
        // Placeholder for leaderboard functionality
        Debug.Log("Leaderboard button clicked - Functionality to be implemented");
    }

    private void OpenCharacterSelection()
    {
        // Placeholder for character selection functionality
        Debug.Log("Character Selection button clicked - Functionality to be implemented");
    }

    private void OpenStore()
    {
        // Placeholder for store functionality
        Debug.Log("Store button clicked - Functionality to be implemented");
    }

    private void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 