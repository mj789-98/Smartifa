using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType
    {
        ShortObject,      // Basic obstacle to jump over
        TallObject,       // Higher obstacle requiring better timing
        GroundHole,       // Gap in the ground to jump over
        HangingObject,    // Must duck or slide under
        MovingObject      // Moves up and down, requiring timing
    }
    
    [Header("Configuration")]
    [Tooltip("Type of obstacle, affects behavior and placement")]
    public ObstacleType obstacleType;
    
    [Header("Movement")]
    [Tooltip("Whether this obstacle should automatically move")]
    public bool isMovingObstacle = false;
    
    [Tooltip("Speed of the obstacle if it's moving")]
    public float movementSpeed = 2f;
    
    [Header("Visuals")]
    [Tooltip("Adjust color based on type for easier debugging")]
    public bool useDebugColors = true;
    
    [Tooltip("Optional mesh/sprite renderer to apply debug colors to")]
    public Renderer obstacleRenderer;
    
    // Private variables
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    void Start()
    {
        initialPosition = transform.position;
        
        // Get renderer if not assigned
        if (obstacleRenderer == null)
        {
            obstacleRenderer = GetComponent<Renderer>();
            
            if (obstacleRenderer == null)
            {
                // Try child renderers
                obstacleRenderer = GetComponentInChildren<Renderer>();
            }
        }
        
        // Get sprite renderer for 2D objects
        spriteRenderer = obstacleRenderer as SpriteRenderer;
        
        // Set type-based debug color
        if (useDebugColors && obstacleRenderer != null)
        {
            SetDebugColor();
        }
        
        // If this is a moving obstacle but not set, auto enable
        if (obstacleType == ObstacleType.MovingObject && !isMovingObstacle)
        {
            isMovingObstacle = true;
        }
    }
    
    void Update()
    {
        // Handle moving obstacle behavior
        if (isMovingObstacle)
        {
            MoveObstacle();
        }
    }
    
    private void MoveObstacle()
    {
        // Simple back-and-forth movement
        float verticalOffset = Mathf.Sin(Time.time * movementSpeed) * 1.5f;
        transform.position = new Vector3(
            initialPosition.x, 
            initialPosition.y + verticalOffset,
            initialPosition.z
        );
    }
    
    private void SetDebugColor()
    {
        // Save original color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else if (obstacleRenderer != null && obstacleRenderer.material != null)
        {
            originalColor = obstacleRenderer.material.color;
        }
        
        // Set color based on type
        Color debugColor = Color.white;
        
        switch (obstacleType)
        {
            case ObstacleType.ShortObject:
                debugColor = new Color(0.2f, 0.6f, 1f); // Light blue
                break;
                
            case ObstacleType.TallObject:
                debugColor = new Color(0.8f, 0.2f, 0.2f); // Red
                break;
                
            case ObstacleType.GroundHole:
                debugColor = new Color(0.2f, 0.2f, 0.6f); // Dark blue
                break;
                
            case ObstacleType.HangingObject:
                debugColor = new Color(0.8f, 0.8f, 0.2f); // Yellow
                break;
                
            case ObstacleType.MovingObject:
                debugColor = new Color(0.8f, 0.2f, 0.8f); // Purple
                break;
        }
        
        // Apply color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = debugColor;
        }
        else if (obstacleRenderer != null && obstacleRenderer.material != null)
        {
            obstacleRenderer.material.color = debugColor;
        }
    }
    
    // Reset color if debug is turned off
    public void DisableDebugColors()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        else if (obstacleRenderer != null && obstacleRenderer.material != null)
        {
            obstacleRenderer.material.color = originalColor;
        }
        
        useDebugColors = false;
    }
    
    // Toggle debug colors
    public void ToggleDebugColors()
    {
        useDebugColors = !useDebugColors;
        
        if (useDebugColors)
        {
            SetDebugColor();
        }
        else
        {
            DisableDebugColors();
        }
    }

    // Used for validating when attaching to a GameObject
    void OnValidate()
    {
        // Auto-set isMovingObstacle based on type
        if (obstacleType == ObstacleType.MovingObject)
        {
            isMovingObstacle = true;
        }
    }
    
    // Optional for drawing debug visuals
    void OnDrawGizmos()
    {
        // Draw debug indicators based on obstacle type
        if (obstacleType == ObstacleType.HangingObject)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3);
        }
        else if (obstacleType == ObstacleType.MovingObject)
        {
            Gizmos.color = Color.magenta;
            Vector3 top = transform.position + Vector3.up * 1.5f;
            Vector3 bottom = transform.position - Vector3.up * 1.5f;
            Gizmos.DrawLine(top, bottom);
            Gizmos.DrawWireSphere(top, 0.2f);
            Gizmos.DrawWireSphere(bottom, 0.2f);
        }
        else if (obstacleType == ObstacleType.GroundHole)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0.5f, 0.1f));
        }
    }
} 