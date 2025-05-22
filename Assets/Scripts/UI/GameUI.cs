using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
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
} 