using UnityEngine;

public enum ObstacleType
{
    Ground,      // Standard ground obstacle
    Floating,    // Floating obstacle that player must jump over/under
    Moving,      // Moving obstacle with custom pattern
    Breakable    // Obstacle that can be destroyed by player
}

[RequireComponent(typeof(BoxCollider2D))]
public class ObstacleTypes : MonoBehaviour
{
    [Header("Obstacle Configuration")]
    public ObstacleType obstacleType = ObstacleType.Ground;
    
    [Header("Moving Obstacle Settings")]
    public bool moveHorizontally;
    public float moveSpeed = 2f;
    public float moveRange = 2f;
    
    [Header("Breakable Settings")]
    public bool isBreakable;
    public int hitsToBreak = 1;
    public GameObject breakEffect;
    
    private Vector3 startPosition;
    private int currentHits;
    private BoxCollider2D boxCollider;
    
    private void Start()
    {
        startPosition = transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Configure collider based on type
        switch (obstacleType)
        {
            case ObstacleType.Floating:
                // Make it a trigger if it's a floating obstacle
                boxCollider.isTrigger = true;
                break;
            case ObstacleType.Breakable:
                isBreakable = true;
                break;
        }
    }
    
    private void Update()
    {
        if (obstacleType == ObstacleType.Moving)
        {
            HandleMovement();
        }
    }
    
    private void HandleMovement()
    {
        if (moveHorizontally)
        {
            float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveRange;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBreakable) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            currentHits++;
            if (currentHits >= hitsToBreak)
            {
                Break();
            }
        }
    }
    
    private void Break()
    {
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    
    // Visualize movement range in editor
    private void OnDrawGizmosSelected()
    {
        if (obstacleType == ObstacleType.Moving)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            if (moveHorizontally)
            {
                Gizmos.DrawWireCube(center, new Vector3(moveRange * 2, 1, 1));
            }
            else
            {
                Gizmos.DrawWireCube(center, new Vector3(1, moveRange * 2, 1));
            }
        }
    }
} 