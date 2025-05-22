using UnityEngine;
using UnityEngine.Events;

public class CoinCollector : MonoBehaviour
{
    [Header("Coin Collection")]
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private float coinCollectVolume = 0.5f;

    [Header("Events")]
    public UnityEvent<int> onCoinsUpdated;
    public UnityEvent<int> onCoinCollected;

    private int totalCoins = 0;
    private AudioSource audioSource;
    private GameManager gameManager;

    private void Start()
    {
        // Set up audio source if needed
        if (coinCollectSound != null && !TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Initialize events if null
        if (onCoinsUpdated == null)
            onCoinsUpdated = new UnityEvent<int>();
        if (onCoinCollected == null)
            onCoinCollected = new UnityEvent<int>();

        // Get GameManager reference
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
        }

        // Trigger initial coins update
        onCoinsUpdated.Invoke(totalCoins);
        if (gameManager != null)
        {
            gameManager.AddCoins(0); // Initialize UI with 0 coins
        }
    }

    public void CollectCoin(int value)
    {
        totalCoins += value;

        // Play sound effect
        if (audioSource != null && coinCollectSound != null)
        {
            audioSource.PlayOneShot(coinCollectSound, coinCollectVolume);
        }

        // Update GameManager
        if (gameManager != null)
        {
            gameManager.AddCoins(value);
        }

        // Trigger events
        onCoinCollected.Invoke(value);
        onCoinsUpdated.Invoke(totalCoins);
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
} 