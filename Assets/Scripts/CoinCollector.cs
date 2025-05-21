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

        // Trigger initial coins update
        onCoinsUpdated.Invoke(totalCoins);
    }

    public void CollectCoin(int value)
    {
        totalCoins += value;

        // Play sound effect
        if (audioSource != null && coinCollectSound != null)
        {
            audioSource.PlayOneShot(coinCollectSound, coinCollectVolume);
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