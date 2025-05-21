using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Settings")]
    public float spawnXOffset = 20f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    
    [Header("Y-Position Settings")]
    public float groundLevel = 0f;
    public float maxHeightAboveGround = 5f;
    public float minHeightAboveGround = 1f;

    [Header("Difficulty Scaling")]
    public float difficultyScalingRate = 0.1f;
    public float minIntervalLimit = 0.5f;
    public float gameStartTime;

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
        }

        gameStartTime = Time.time;
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Check if it's time to spawn a new obstacle
        if (Time.time > nextSpawnTime)
        {
            SpawnObstacle();
            UpdateSpawnInterval();
        }
    }

    private void SpawnObstacle()
    {
        // Select random obstacle prefab
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);
        GameObject selectedPrefab = obstaclePrefabs[randomIndex];

        // Get spawn position
        Vector2 spawnPosition = new Vector2(
            playerTransform.position.x + spawnXOffset,
            GetRandomYPosition(selectedPrefab)
        );

        // Spawn the obstacle
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Set next spawn time
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private float GetRandomYPosition(GameObject obstacle)
    {
        // Get the obstacle's collider height (if any)
        BoxCollider2D collider = obstacle.GetComponent<BoxCollider2D>();
        float objectHeight = collider != null ? collider.size.y : 1f;

        // Determine if this should be a ground-level or floating obstacle
        bool isGroundObstacle = Random.value > 0.3f; // 70% chance for ground obstacles

        if (isGroundObstacle)
        {
            return groundLevel + (objectHeight * 0.5f);
        }
        else
        {
            // For floating obstacles, ensure they're at a jumpable height
            return Random.Range(minHeightAboveGround, maxHeightAboveGround);
        }
    }

    private void UpdateSpawnInterval()
    {
        // Calculate time-based difficulty scaling
        float timeElapsed = Time.time - gameStartTime;
        float difficultyMultiplier = 1f - (timeElapsed * difficultyScalingRate);
        
        // Clamp the values to prevent them from becoming too small
        minSpawnInterval = Mathf.Max(minIntervalLimit, minSpawnInterval * difficultyMultiplier);
        maxSpawnInterval = Mathf.Max(minSpawnInterval + 0.5f, maxSpawnInterval * difficultyMultiplier);
    }

    // Optional: Visualize spawn area in editor
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.green;
            Vector3 spawnAreaCenter = new Vector3(
                playerTransform.position.x + spawnXOffset,
                groundLevel + maxHeightAboveGround * 0.5f,
                0
            );
            Vector3 spawnAreaSize = new Vector3(1, maxHeightAboveGround, 1);
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
    }
} 