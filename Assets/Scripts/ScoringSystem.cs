using UnityEngine;
using UnityEngine.Events;

public class ScoringSystem : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private float distanceScoreMultiplier = 1f;
    [SerializeField] private int coinScoreValue = 100;
    [SerializeField] private float comboTimeWindow = 2f;
    [SerializeField] private float comboMultiplierIncrement = 0.5f;
    [SerializeField] private float maxComboMultiplier = 4f;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Events")]
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<float> onComboMultiplierChanged;
    public UnityEvent<int> onCoinCollected;
    public UnityEvent<int> onHighScoreReached;

    private int currentScore;
    private int highScore;
    private float startXPosition;
    private float comboTimer;
    private float currentComboMultiplier = 1f;
    private GameOverManager gameOverManager;
    private LivesSystem livesSystem;
    private GameManager gameManager;

    private void Awake()
    {
        // Initialize events
        if (onScoreChanged == null) onScoreChanged = new UnityEvent<int>();
        if (onComboMultiplierChanged == null) onComboMultiplierChanged = new UnityEvent<float>();
        if (onCoinCollected == null) onCoinCollected = new UnityEvent<int>();
        if (onHighScoreReached == null) onHighScoreReached = new UnityEvent<int>();

        // Get components
        gameOverManager = FindObjectOfType<GameOverManager>();
        livesSystem = FindObjectOfType<LivesSystem>();
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }

        // Auto-find player if not assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameLayers.PlayerTag);
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player not found! Ensure player has the correct tag.");
            }
        }

        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        InitializeScore();
    }

    private void Update()
    {
        if (livesSystem != null && livesSystem.IsGameOver()) return;

        // Update distance-based score
        UpdateDistanceScore();

        // Update combo timer
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    public void InitializeScore()
    {
        currentScore = 0;
        startXPosition = playerTransform != null ? playerTransform.position.x : 0f;
        ResetCombo();
        onScoreChanged.Invoke(currentScore);
        
        // Update GameManager
        if (gameManager != null)
        {
            gameManager.UpdateScore(currentScore);
        }
    }

    private void UpdateDistanceScore()
    {
        if (playerTransform == null) return;

        float distanceTraveled = playerTransform.position.x - startXPosition;
        int newDistanceScore = Mathf.FloorToInt(distanceTraveled * distanceScoreMultiplier);
        
        if (newDistanceScore != currentScore)
        {
            currentScore = newDistanceScore;
            onScoreChanged.Invoke(currentScore);
            
            // Update GameManager
            if (gameManager != null)
            {
                gameManager.UpdateScore(currentScore);
            }
            
            CheckHighScore();
        }
    }

    public void AddCoinScore()
    {
        // Add base coin score multiplied by combo
        int coinScore = Mathf.RoundToInt(coinScoreValue * currentComboMultiplier);
        currentScore += coinScore;
        
        // Update combo
        currentComboMultiplier = Mathf.Min(currentComboMultiplier + comboMultiplierIncrement, maxComboMultiplier);
        comboTimer = comboTimeWindow;
        
        // Trigger events
        onScoreChanged.Invoke(currentScore);
        onCoinCollected.Invoke(coinScore);
        onComboMultiplierChanged.Invoke(currentComboMultiplier);
        
        // Update GameManager
        if (gameManager != null)
        {
            gameManager.UpdateScore(currentScore);
        }
        
        CheckHighScore();
    }

    private void ResetCombo()
    {
        currentComboMultiplier = 1f;
        comboTimer = 0f;
        onComboMultiplierChanged.Invoke(currentComboMultiplier);
    }

    private void CheckHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            onHighScoreReached.Invoke(highScore);
        }

        // Update game over manager
        if (gameOverManager != null)
        {
            gameOverManager.SetCurrentScore(currentScore);
        }
    }

    // Public getters
    public int GetCurrentScore() => currentScore;
    public int GetHighScore() => highScore;
    public float GetComboMultiplier() => currentComboMultiplier;
    public float GetComboTimeRemaining() => comboTimer;
} 