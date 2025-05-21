using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private float destroyXOffset = -20f; // Distance behind player to destroy
    [SerializeField] private bool isMovingObstacle;
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float movementRange = 2f;
    
    private Vector3 startPosition;
    private float initialY;
    private Transform playerTransform;

    private void Start()
    {
        // Cache the initial position for moving obstacles
        startPosition = transform.position;
        initialY = transform.position.y;
        
        // Find the player - assuming it has a "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Check if obstacle is too far behind the player
        if (transform.position.x < playerTransform.position.x + destroyXOffset)
        {
            Destroy(gameObject);
            return;
        }

        // Handle moving obstacle behavior
        if (isMovingObstacle)
        {
            // Simple up and down movement using sin wave
            float newY = initialY + Mathf.Sin(Time.time * movementSpeed) * movementRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // You can add specific collision behavior here
        // For example, if you want some obstacles to break on collision
        if (collision.gameObject.CompareTag("Player"))
        {
            // Add any specific behavior when player collides
            Debug.Log($"Player collided with obstacle: {gameObject.name}");
        }
    }

    // Optional: Visualize the movement range in the editor
    private void OnDrawGizmosSelected()
    {
        if (isMovingObstacle)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawWireCube(center, new Vector3(1, movementRange * 2, 1));
        }
    }
} 