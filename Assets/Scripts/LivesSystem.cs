using UnityEngine;
using UnityEngine.Events;

public class LivesSystem : MonoBehaviour
{
    [Header("Lives Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private bool canGainExtraLives = true;
    [SerializeField] private int maxExtraLives = 5;

    [Header("Recovery")]
    [SerializeField] private bool enableAutoRecovery = false;
    [SerializeField] private float recoveryInterval = 60f;
    [SerializeField] private int recoveryAmount = 1;

    [Header("Effects")]
    [SerializeField] private AudioClip loseLifeSound;
    [SerializeField] private AudioClip gainLifeSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private GameObject loseLifeEffect;
    [SerializeField] private GameObject gainLifeEffect;

    [Header("Events")]
    public UnityEvent<int> onLivesChanged;
    public UnityEvent<int> onLifeLost;
    public UnityEvent<int> onLifeGained;
    public UnityEvent onGameOver;
    public UnityEvent onMaxLivesReached;

    private int currentLives;
    private float recoveryTimer;
    private AudioSource audioSource;
    private CollisionHandler collisionHandler;
    private bool isGameOver = false;

    private void Awake()
    {
        // Get components
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (loseLifeSound != null || gainLifeSound != null || gameOverSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        collisionHandler = GetComponent<CollisionHandler>();
        if (collisionHandler != null)
        {
            collisionHandler.onObstacleCollision.AddListener(OnObstacleHit);
        }

        // Initialize events
        if (onLivesChanged == null) onLivesChanged = new UnityEvent<int>();
        if (onLifeLost == null) onLifeLost = new UnityEvent<int>();
        if (onLifeGained == null) onLifeGained = new UnityEvent<int>();
        if (onGameOver == null) onGameOver = new UnityEvent();
        if (onMaxLivesReached == null) onMaxLivesReached = new UnityEvent();
    }

    private void Start()
    {
        InitializeLives();
    }

    private void Update()
    {
        if (isGameOver || !enableAutoRecovery || currentLives >= maxLives) return;

        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= recoveryInterval)
        {
            recoveryTimer = 0f;
            AddLives(recoveryAmount);
        }
    }

    public void InitializeLives()
    {
        currentLives = maxLives;
        isGameOver = false;
        recoveryTimer = 0f;
        onLivesChanged.Invoke(currentLives);
    }

    public void LoseLife()
    {
        if (isGameOver) return;

        currentLives--;
        onLivesChanged.Invoke(currentLives);
        onLifeLost.Invoke(currentLives);

        if (audioSource != null && loseLifeSound != null)
        {
            audioSource.PlayOneShot(loseLifeSound);
        }

        if (loseLifeEffect != null)
        {
            Instantiate(loseLifeEffect, transform.position, Quaternion.identity);
        }

        if (currentLives <= 0)
        {
            TriggerGameOver();
        }
    }

    public void AddLives(int amount)
    {
        if (isGameOver || !canGainExtraLives || currentLives >= maxExtraLives) return;

        int oldLives = currentLives;
        currentLives = Mathf.Min(currentLives + amount, maxExtraLives);

        if (currentLives > oldLives)
        {
            onLivesChanged.Invoke(currentLives);
            onLifeGained.Invoke(currentLives);

            if (audioSource != null && gainLifeSound != null)
            {
                audioSource.PlayOneShot(gainLifeSound);
            }

            if (gainLifeEffect != null)
            {
                Instantiate(gainLifeEffect, transform.position, Quaternion.identity);
            }

            if (currentLives >= maxExtraLives)
            {
                onMaxLivesReached.Invoke();
            }
        }
    }

    private void OnObstacleHit(GameObject obstacle)
    {
        LoseLife();
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Stop the game
        Time.timeScale = 0f;

        // Trigger game over event
        onGameOver.Invoke();
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        InitializeLives();
    }

    // Public getters
    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;
    public bool IsGameOver() => isGameOver;
    public float GetRecoveryProgress() => enableAutoRecovery ? recoveryTimer / recoveryInterval : 0f;
} 