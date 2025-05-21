using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObstacleDebugger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the ObstacleManager")]
    public ObstacleManager obstacleManager;
    
    [Tooltip("The player's transform")]
    public Transform playerTransform;
    
    [Header("Visualization")]
    [Tooltip("Show debug gizmos for obstacle spawning")]
    public bool showGizmos = true;
    
    [Tooltip("Color for likely jump paths")]
    public Color jumpPathColor = new Color(0f, 1f, 0f, 0.3f);
    
    [Header("Testing Controls")]
    [Tooltip("Force difficulty increase for testing")]
    public bool forceDifficultyIncrease = false;
    
    [Tooltip("Force spawn of all obstacle types for testing")]
    public bool spawnTestObstacles = false;
    
    [Tooltip("Test distance in front of the player to spawn test obstacles")]
    public float testSpawnDistance = 10f;
    
    [Header("UI (Optional)")]
    [Tooltip("Text component to display debug information")]
    public Text debugText;

    // Internal tracking
    private float timeToNextDebugUpdate = 0f;
    private const float debugUpdateInterval = 0.5f;
    private int obstacleCount = 0;
    private Dictionary<string, int> obstacleTypeCount = new Dictionary<string, int>();
    
    void Start()
    {
        // Auto-find references if not set
        if (obstacleManager == null)
        {
            obstacleManager = FindObjectOfType<ObstacleManager>();
        }
        
        if (playerTransform == null && Camera.main != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        // Initialize obstacle type counter
        InitializeObstacleTypeCounter();
    }
    
    void Update()
    {
        // Update debug information
        timeToNextDebugUpdate -= Time.deltaTime;
        if (timeToNextDebugUpdate <= 0f)
        {
            CountObstacles();
            UpdateDebugText();
            timeToNextDebugUpdate = debugUpdateInterval;
        }
        
        // Testing controls
        if (forceDifficultyIncrease && Input.GetKeyDown(KeyCode.D))
        {
            IncreaseDifficulty();
        }
        
        if (spawnTestObstacles && Input.GetKeyDown(KeyCode.T))
        {
            SpawnAllObstacleTypes();
        }
    }
    
    private void InitializeObstacleTypeCounter()
    {
        obstacleTypeCount.Clear();
        
        if (obstacleManager != null && obstacleManager.obstaclePrefabs != null)
        {
            foreach (var prefab in obstacleManager.obstaclePrefabs)
            {
                if (prefab != null)
                {
                    Obstacle obstacle = prefab.GetComponent<Obstacle>();
                    if (obstacle != null)
                    {
                        string typeName = obstacle.obstacleType.ToString();
                        if (!obstacleTypeCount.ContainsKey(typeName))
                        {
                            obstacleTypeCount.Add(typeName, 0);
                        }
                    }
                }
            }
        }
    }
    
    private void CountObstacles()
    {
        if (obstacleManager == null) return;
        
        // Reset counters
        obstacleCount = 0;
        foreach (var key in obstacleTypeCount.Keys)
        {
            obstacleTypeCount[key] = 0;
        }
        
        // Count all active obstacles
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        obstacleCount = obstacles.Length;
        
        foreach (var obstacle in obstacles)
        {
            string typeName = obstacle.obstacleType.ToString();
            if (obstacleTypeCount.ContainsKey(typeName))
            {
                obstacleTypeCount[typeName]++;
            }
        }
    }
    
    private void UpdateDebugText()
    {
        if (debugText == null) return;
        
        string text = "Obstacle Debug:\n";
        text += $"Active Obstacles: {obstacleCount}\n";
        
        // Add obstacle type counts
        text += "Types:\n";
        foreach (var pair in obstacleTypeCount)
        {
            text += $"- {pair.Key}: {pair.Value}\n";
        }
        
        // Add difficulty info if available
        if (obstacleManager != null)
        {
            text += $"\nSpawn Interval: {obstacleManager.minSpawnInterval:F2}s - {obstacleManager.maxSpawnInterval:F2}s\n";
            
            // Reflect other difficulty values using reflection
            var propSpacing = typeof(ObstacleManager).GetField("minObstacleSpacing");
            if (propSpacing != null)
            {
                float spacing = (float)propSpacing.GetValue(obstacleManager);
                text += $"Min Spacing: {spacing:F2}\n";
            }
            
            var propYVar = typeof(ObstacleManager).GetField("yVariationProbability");
            if (propYVar != null)
            {
                float yVar = (float)propYVar.GetValue(obstacleManager);
                text += $"Y Variation: {yVar:F2}\n";
            }
            
            var propGroups = typeof(ObstacleManager).GetField("obstacleGroupProbability");
            if (propGroups != null)
            {
                float groupProb = (float)propGroups.GetValue(obstacleManager);
                text += $"Group Probability: {groupProb:F2}\n";
            }
        }
        
        debugText.text = text;
    }
    
    private void IncreaseDifficulty()
    {
        if (obstacleManager == null) return;
        
        // Try to call IncreaseDifficulty through reflection
        var method = typeof(ObstacleManager).GetMethod("IncreaseDifficulty", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            
        if (method != null)
        {
            method.Invoke(obstacleManager, new object[] { "Debug-forced" });
            Debug.Log("Forced difficulty increase");
        }
        else
        {
            Debug.LogWarning("Could not force difficulty increase - method not found");
        }
    }
    
    private void SpawnAllObstacleTypes()
    {
        if (obstacleManager == null || playerTransform == null) return;
        
        float spawnX = playerTransform.position.x + testSpawnDistance;
        float spacing = 2.5f;
        float spawnY = obstacleManager.groundLevel;
        
        if (obstacleManager.obstaclePrefabs != null)
        {
            for (int i = 0; i < obstacleManager.obstaclePrefabs.Length; i++)
            {
                GameObject prefab = obstacleManager.obstaclePrefabs[i];
                if (prefab != null)
                {
                    Vector3 position = new Vector3(spawnX + (i * spacing), spawnY, 0);
                    
                    // Use Y position logic from obstacle manager for appropriate height
                    Obstacle obstacle = prefab.GetComponent<Obstacle>();
                    if (obstacle != null)
                    {
                        if (obstacle.obstacleType == Obstacle.ObstacleType.HangingObject)
                        {
                            position.y = spawnY + 2f;
                        }
                        else if (obstacle.obstacleType == Obstacle.ObstacleType.GroundHole)
                        {
                            position.y = spawnY - 0.5f;
                        }
                        else if (obstacle.obstacleType == Obstacle.ObstacleType.MovingObject)
                        {
                            position.y = spawnY + 1f;
                        }
                    }
                    
                    GameObject spawnedObstacle = Instantiate(prefab, position, Quaternion.identity);
                    Debug.Log($"Test spawned: {prefab.name} at {position}");
                    
                    // Parent to obstacle manager if possible
                    if (obstacleManager.transform != null)
                    {
                        spawnedObstacle.transform.parent = obstacleManager.transform;
                    }
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmos || obstacleManager == null || playerTransform == null) return;
        
        // Draw expected jump trajectory
        float jumpHeight = 2.5f; // Estimated jump height
        float jumpDistance = 5f; // Estimated jump distance
        
        Vector3 start = new Vector3(playerTransform.position.x, obstacleManager.groundLevel, 0);
        Vector3 peak = new Vector3(playerTransform.position.x + (jumpDistance / 2f), 
                                  obstacleManager.groundLevel + jumpHeight, 0);
        Vector3 end = new Vector3(playerTransform.position.x + jumpDistance, 
                                 obstacleManager.groundLevel, 0);
        
        // Draw jump arc using Bezier curve
        Gizmos.color = jumpPathColor;
        DrawBezierCurve(start, peak, peak, end, 20);
        
        // Draw forced debug spawn location
        if (spawnTestObstacles)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(new Vector3(playerTransform.position.x + testSpawnDistance, 
                                             obstacleManager.groundLevel, 0), 1f);
        }
    }
    
    private void DrawBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentCount)
    {
        Vector3 prevPoint = p0;
        
        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 point = Mathf.Pow(1 - t, 3) * p0 +
                           3 * Mathf.Pow(1 - t, 2) * t * p1 +
                           3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                           Mathf.Pow(t, 3) * p3;
                           
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
    #endif
} 