using UnityEngine;
using System.Collections.Generic;

public class GroundGenerator : MonoBehaviour
{
    [Header("Ground Settings")]
    [Tooltip("Width of each ground segment")]
    public float segmentWidth = 10f;
    
    [Tooltip("Height of the ground")]
    public float groundHeight = 1f;
    
    [Tooltip("How many segments to keep ahead of the player")]
    public int segmentsAhead = 3;
    
    [Tooltip("How many segments to keep behind the player")]
    public int segmentsBehind = 2;
    
    [Tooltip("Reference to the player transform")]
    public Transform playerTransform;

    [Header("Debug")]
    [Tooltip("Show debug logs")]
    public bool showDebug = true;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float lastGeneratedX = 0f;
    private int groundLayer = 0;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to GroundGenerator! Please assign a Player Transform in the inspector.");
            enabled = false;
            return;
        }

        // Find or create ground layer
        SetupGroundLayer();

        // Create initial ground segments
        for (int i = 0; i < segmentsAhead + segmentsBehind; i++)
        {
            float xPos = (i - segmentsBehind) * segmentWidth;
            CreateGroundSegment(xPos);
            if (i == segmentsAhead + segmentsBehind - 1)
            {
                lastGeneratedX = xPos;
            }
        }
        
        // Create a starting platform directly below the player
        CreateStartPlatform();
        
        if (showDebug)
        {
            Debug.Log($"GroundGenerator initialized with {activeSegments.Count} segments. Last X: {lastGeneratedX}");
        }
    }

    private void CreateStartPlatform()
    {
        // Create a platform directly under the player's starting position
        GameObject startPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        startPlatform.name = "Start_Platform";
        startPlatform.transform.parent = transform;
        startPlatform.transform.position = new Vector3(0, -0.5f, 0);
        startPlatform.transform.localScale = new Vector3(segmentWidth, 1f, 1f);
        
        // Set up proper 2D collider
        BoxCollider originalCollider = startPlatform.GetComponent<BoxCollider>();
        if (originalCollider != null)
        {
            DestroyImmediate(originalCollider);
            BoxCollider2D collider2D = startPlatform.AddComponent<BoxCollider2D>();
            collider2D.size = new Vector2(segmentWidth, 1f);
            collider2D.offset = Vector2.zero;
            collider2D.isTrigger = false;
        }
        
        // Set to ground layer
        startPlatform.layer = groundLayer;
        
        Debug.Log($"Created starting platform at position: {startPlatform.transform.position}, layer: {startPlatform.layer}, with layer name: {LayerMask.LayerToName(startPlatform.layer)}");
        
        // Add to active segments list
        activeSegments.Add(startPlatform);
    }

    private void SetupGroundLayer()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
            Debug.LogWarning("'Ground' layer not found. Please create a layer named 'Ground' in the Unity Editor.");
            groundLayer = 0; // Default
            Debug.Log($"Using Default layer (0) for ground segments");
        }
        else
        {
            Debug.Log($"Using Ground layer: {groundLayer}");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float playerX = playerTransform.position.x;
        
        // Generate segments ahead
        while (lastGeneratedX < playerX + (segmentWidth * segmentsAhead))
        {
            lastGeneratedX += segmentWidth;
            CreateGroundSegment(lastGeneratedX);
            
            if (showDebug)
            {
                Debug.Log($"Created new segment at X: {lastGeneratedX}");
            }
        }

        // Cleanup segments behind
        CleanupOldSegments();
    }

    private void CreateGroundSegment(float xPosition)
    {
        // Create a new ground segment
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = $"Ground_Segment_{xPosition}";
        segment.transform.parent = transform;
        segment.transform.position = new Vector3(xPosition, -0.5f, 0f);
        segment.transform.localScale = new Vector3(segmentWidth, groundHeight, 1f);

        // Set up proper 2D collider
        BoxCollider originalCollider = segment.GetComponent<BoxCollider>();
        if (originalCollider != null)
        {
            // Store the original collider's size before destroying it
            Vector3 colliderSize = originalCollider.size;
            Vector3 colliderCenter = originalCollider.center;
            
            // Remove the 3D collider
            DestroyImmediate(originalCollider);

            // Add and set up the 2D collider with same dimensions
            BoxCollider2D collider2D = segment.AddComponent<BoxCollider2D>();
            collider2D.size = new Vector2(colliderSize.x, colliderSize.y);
            collider2D.offset = new Vector2(colliderCenter.x, colliderCenter.y);
            
            // Ensure it's not a trigger
            collider2D.isTrigger = false;
            
            if (showDebug)
            {
                Debug.Log($"Created 2D collider for segment at X: {xPosition}, Size: {collider2D.size}");
            }
        }

        // Set the layer to Ground
        segment.layer = groundLayer;
        
        // Add a simple material for visibility
        Renderer rend = segment.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = new Color(0.5f, 0.35f, 0.2f);
        }

        // Add to active segments list
        activeSegments.Add(segment);
        
        if (showDebug)
        {
            Debug.Log($"Set segment layer to {segment.layer} ({LayerMask.LayerToName(segment.layer)})");
        }
    }

    private void CleanupOldSegments()
    {
        float playerX = playerTransform.position.x;
        float minX = playerX - (segmentWidth * segmentsBehind);

        int removed = 0;
        // Remove segments that are too far behind
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            if (activeSegments[i] == null) 
            {
                activeSegments.RemoveAt(i);
                continue;
            }

            if (activeSegments[i].transform.position.x < minX)
            {
                GameObject segment = activeSegments[i];
                activeSegments.RemoveAt(i);
                Destroy(segment);
                removed++;
            }
        }
        
        if (removed > 0 && showDebug)
        {
            Debug.Log($"Removed {removed} ground segments behind player. Active segments: {activeSegments.Count}");
        }
    }

    private void OnDrawGizmos()
    {
        // Draw boundaries for generation/cleanup
        if (playerTransform != null)
        {
            float playerX = playerTransform.position.x;
            
            // Draw ahead boundary
            Gizmos.color = Color.green;
            Vector3 aheadPos = new Vector3(playerX + (segmentWidth * segmentsAhead), 0, 0);
            Gizmos.DrawLine(aheadPos + Vector3.up * 5, aheadPos + Vector3.down * 5);
            
            // Draw behind boundary
            Gizmos.color = Color.red;
            Vector3 behindPos = new Vector3(playerX - (segmentWidth * segmentsBehind), 0, 0);
            Gizmos.DrawLine(behindPos + Vector3.up * 5, behindPos + Vector3.down * 5);
        }
    }
} 