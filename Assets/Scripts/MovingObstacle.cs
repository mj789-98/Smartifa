using UnityEngine;

[RequireComponent(typeof(Obstacle))]
public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How far the obstacle moves up and down")]
    public float movementMagnitude = 1.5f;
    
    [Tooltip("How quickly the obstacle moves")]
    public float movementSpeed = 2.0f;
    
    private Vector3 startPosition;
    private float offsetValue;
    
    private void Start()
    {
        // Store the initial position
        startPosition = transform.position;
        
        // Random offset to prevent all moving obstacles moving in sync
        offsetValue = Random.Range(0f, 2f * Mathf.PI);
    }
    
    private void Update()
    {
        // Calculate vertical movement using sine wave
        float newY = startPosition.y + Mathf.Sin((Time.time * movementSpeed) + offsetValue) * movementMagnitude;
        
        // Apply new position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
} 