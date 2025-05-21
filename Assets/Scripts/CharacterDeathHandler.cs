using UnityEngine;

public class CharacterDeathHandler : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision is with an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Trigger game over in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
} 