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
    private int obstacleLayer;

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

        // Get the obstacle layer
        obstacleLayer = LayerMask.NameToLayer(GameLayers.ObstacleLayer);
        if (obstacleLayer == -1)
        {
            Debug.LogError($"Layer '{GameLayers.ObstacleLayer}' not found! Please create this layer in Unity.");
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
        GameObject obstacle = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Set the layer and tag
        GameLayers.SetupObjectLayers(obstacle, GameLayers.ObstacleLayer, GameLayers.ObstacleTag);

        // Debug log
        Debug.Log($"Spawned obstacle: {obstacle.name}, Layer: {obstacle.layer}, Tag: {obstacle.tag}");
    }

    private float GetRandomYPosition(GameObject prefab)
    {
        // Get the height of the obstacle using its collider
        BoxCollider2D collider = prefab.GetComponent<BoxCollider2D>();
        float obstacleHeight = collider != null ? collider.size.y * prefab.transform.localScale.y : 1f;

        // Calculate min and max Y positions
        float minY = groundLevel + (obstacleHeight * 0.5f) + minHeightAboveGround;
        float maxY = groundLevel + maxHeightAboveGround - (obstacleHeight * 0.5f);

        // Return random Y position within range
        return Random.Range(minY, maxY);
    }

    private void UpdateSpawnInterval()
    {
        // Calculate time since game start
        float timeSinceStart = Time.time - gameStartTime;

        // Calculate new interval range based on difficulty scaling
        float currentMinInterval = Mathf.Max(minSpawnInterval - (timeSinceStart * difficultyScalingRate), minIntervalLimit);
        float currentMaxInterval = Mathf.Max(maxSpawnInterval - (timeSinceStart * difficultyScalingRate), currentMinInterval + 0.5f);

        // Set next spawn time
        nextSpawnTime = Time.time + Random.Range(currentMinInterval, currentMaxInterval);
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