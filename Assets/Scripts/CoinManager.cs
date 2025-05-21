using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float spawnXOffset = 20f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;

    [Header("Pattern Settings")]
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private int minCoinsInPattern = 3;
    [SerializeField] private int maxCoinsInPattern = 7;
    [SerializeField] private float coinSpacing = 1f;

    private float nextSpawnTime;
    private Transform playerTransform;

    private void Start()
    {
        // Find and cache the player reference
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found! Ensure player has 'Player' tag.");
            enabled = false;
            return;
        }

        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        if (Time.time > nextSpawnTime)
        {
            SpawnCoinPattern();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnCoinPattern()
    {
        // Determine pattern type
        int patternType = Random.Range(0, 4); // 0: Line, 1: Arc, 2: Zigzag, 3: Random
        int coinsInPattern = Random.Range(minCoinsInPattern, maxCoinsInPattern + 1);

        switch (patternType)
        {
            case 0: // Line pattern
                SpawnLinePattern(coinsInPattern);
                break;
            case 1: // Arc pattern
                SpawnArcPattern(coinsInPattern);
                break;
            case 2: // Zigzag pattern
                SpawnZigzagPattern(coinsInPattern);
                break;
            case 3: // Random pattern
                SpawnRandomPattern(coinsInPattern);
                break;
        }
    }

    private void SpawnLinePattern(int count)
    {
        float startX = playerTransform.position.x + spawnXOffset;
        float height = Random.Range(minHeight, maxHeight);

        for (int i = 0; i < count; i++)
        {
            Vector3 position = new Vector3(startX + (i * coinSpacing), height, 0);
            SpawnCoin(position);
        }
    }

    private void SpawnArcPattern(int count)
    {
        float startX = playerTransform.position.x + spawnXOffset;
        float centerHeight = Random.Range(minHeight, maxHeight);
        float radius = count * coinSpacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float progress = (float)i / (count - 1);
            float angle = Mathf.PI * progress;
            float x = startX + (i * coinSpacing);
            float y = centerHeight + Mathf.Sin(angle) * radius * 0.5f;
            SpawnCoin(new Vector3(x, y, 0));
        }
    }

    private void SpawnZigzagPattern(int count)
    {
        float startX = playerTransform.position.x + spawnXOffset;
        float centerHeight = Random.Range(minHeight, maxHeight);
        float amplitude = (maxHeight - minHeight) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + (i * coinSpacing);
            float y = centerHeight + (i % 2 == 0 ? amplitude : -amplitude);
            SpawnCoin(new Vector3(x, y, 0));
        }
    }

    private void SpawnRandomPattern(int count)
    {
        float startX = playerTransform.position.x + spawnXOffset;

        for (int i = 0; i < count; i++)
        {
            float x = startX + (i * coinSpacing);
            float y = Random.Range(minHeight, maxHeight);
            SpawnCoin(new Vector3(x, y, 0));
        }
    }

    private void SpawnCoin(Vector3 position)
    {
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            Debug.LogError("Coin prefab not assigned to CoinManager!");
        }
    }

    // Optional: Visualize spawn area in editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 spawnAreaCenter = new Vector3(
                playerTransform.position.x + spawnXOffset,
                (maxHeight + minHeight) * 0.5f,
                0
            );
            Vector3 spawnAreaSize = new Vector3(maxCoinsInPattern * coinSpacing, maxHeight - minHeight, 1);
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
    }
} 