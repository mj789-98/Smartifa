using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over Settings")]
    [SerializeField] private float gameOverTransitionDuration = 1f;
    [SerializeField] private float scoreDisplayDelay = 0.5f;
    [SerializeField] private float highScoreDisplayDelay = 1f;
    
    [Header("Effects")]
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private float musicFadeInDuration = 1f;
    [SerializeField] private float gameplayMusicFadeOutDuration = 0.5f;
    [SerializeField] private GameObject gameOverEffect;
    [SerializeField] private GameObject confettiEffect;  // For new high score

    [Header("References")]
    [SerializeField] private AudioSource gameplayMusicSource;
    [SerializeField] private AudioSource gameOverMusicSource;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameplayUI;

    [Header("Events")]
    public UnityEvent onGameOverStart;
    public UnityEvent onGameOverComplete;
    public UnityEvent<int> onFinalScoreDisplay;
    public UnityEvent<int> onHighScoreDisplay;
    public UnityEvent onNewHighScore;
    public UnityEvent onRestartGame;

    private LivesSystem livesSystem;
    private bool isTransitioning = false;
    private int currentScore = 0;
    private int highScore = 0;

    private void Awake()
    {
        // Get components
        livesSystem = FindObjectOfType<LivesSystem>();
        
        if (livesSystem != null)
        {
            livesSystem.onGameOver.AddListener(HandleGameOver);
        }
        else
        {
            Debug.LogError("LivesSystem not found in scene!");
        }

        // Initialize events
        if (onGameOverStart == null) onGameOverStart = new UnityEvent();
        if (onGameOverComplete == null) onGameOverComplete = new UnityEvent();
        if (onFinalScoreDisplay == null) onFinalScoreDisplay = new UnityEvent<int>();
        if (onHighScoreDisplay == null) onHighScoreDisplay = new UnityEvent<int>();
        if (onNewHighScore == null) onNewHighScore = new UnityEvent();
        if (onRestartGame == null) onRestartGame = new UnityEvent();

        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        // Ensure game over UI is hidden at start
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void HandleGameOver()
    {
        if (isTransitioning) return;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        isTransitioning = true;
        onGameOverStart.Invoke();

        // Spawn game over effect
        if (gameOverEffect != null)
        {
            Instantiate(gameOverEffect, Camera.main.transform.position + Vector3.forward * 2f, Quaternion.identity);
        }

        // Fade out gameplay music
        if (gameplayMusicSource != null)
        {
            StartCoroutine(FadeAudioSource(gameplayMusicSource, 0f, gameplayMusicFadeOutDuration));
        }

        // Wait for initial transition
        yield return new WaitForSecondsRealtime(gameOverTransitionDuration);

        // Switch UI
        if (gameplayUI != null)
            gameplayUI.SetActive(false);
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Start game over music
        if (gameOverMusicSource != null && gameOverMusic != null)
        {
            gameOverMusicSource.clip = gameOverMusic;
            gameOverMusicSource.volume = 0f;
            gameOverMusicSource.Play();
            StartCoroutine(FadeAudioSource(gameOverMusicSource, 1f, musicFadeInDuration));
        }

        // Display final score
        yield return new WaitForSecondsRealtime(scoreDisplayDelay);
        onFinalScoreDisplay.Invoke(currentScore);

        // Check and display high score
        yield return new WaitForSecondsRealtime(highScoreDisplayDelay);
        
        bool isNewHighScore = currentScore > highScore;
        if (isNewHighScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            
            if (confettiEffect != null)
            {
                Instantiate(confettiEffect, Camera.main.transform.position + Vector3.forward * 2f, Quaternion.identity);
            }
            
            onNewHighScore.Invoke();
        }
        
        onHighScoreDisplay.Invoke(highScore);

        // Complete transition
        isTransitioning = false;
        onGameOverComplete.Invoke();
    }

    public void RestartGame()
    {
        if (isTransitioning) return;

        // Reset UI
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        // Reset music
        if (gameOverMusicSource != null)
            gameOverMusicSource.Stop();
        if (gameplayMusicSource != null)
        {
            gameplayMusicSource.volume = 1f;
            if (!gameplayMusicSource.isPlaying)
                gameplayMusicSource.Play();
        }

        // Reset game state
        Time.timeScale = 1f;
        currentScore = 0;

        // Reset lives system
        if (livesSystem != null)
            livesSystem.ResetGame();

        onRestartGame.Invoke();
    }

    private IEnumerator FadeAudioSource(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    // Public methods for external access
    public void SetCurrentScore(int score) => currentScore = score;
    public int GetHighScore() => highScore;
    public bool IsTransitioning() => isTransitioning;
} 