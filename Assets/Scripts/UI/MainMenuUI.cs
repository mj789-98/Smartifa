using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu")]
    public Button startGameButton;
    public Button settingsButton;
    public Button leaderboardButton;
    public Button exitButton;
    public Button characterSelectionButton;
    public Button storeButton;

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
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // Ensure normal time scale
        SceneManager.LoadScene("GameScene"); // Make sure to set up this scene name in build settings
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