using UnityEngine;
using UnityEngine.Events;

public class CollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private float invincibilityDuration = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float hitSoundVolume = 1f;

    [Header("Events")]
    public UnityEvent<GameObject> onObstacleCollision;
    public UnityEvent onInvincibilityStart;
    public UnityEvent onInvincibilityEnd;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private Color originalColor;
    private float knockbackTimer = 0f;
    private bool isKnockedBack = false;
    private Vector2 knockbackDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null && hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        // Initialize events if null
        if (onObstacleCollision == null)
            onObstacleCollision = new UnityEvent<GameObject>();
        if (onInvincibilityStart == null)
            onInvincibilityStart = new UnityEvent();
        if (onInvincibilityEnd == null)
            onInvincibilityEnd = new UnityEvent();
    }

    private void Update()
    {
        // Handle invincibility
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                EndInvincibility();
            }
            else if (spriteRenderer != null)
            {
                // Flash effect during invincibility
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
        }

        // Handle knockback
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                EndKnockback();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;

        if (collision.gameObject.CompareTag(GameLayers.ObstacleTag))
        {
            HandleObstacleCollision(collision.gameObject, collision.GetContact(0).normal);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible) return;

        if (other.CompareTag(GameLayers.ObstacleTag))
        {
            Vector2 direction = (transform.position - other.transform.position).normalized;
            HandleObstacleCollision(other.gameObject, direction);
        }
    }

    private void HandleObstacleCollision(GameObject obstacle, Vector2 collisionNormal)
    {
        // Trigger invincibility
        StartInvincibility();

        // Apply knockback
        ApplyKnockback(collisionNormal);

        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, hitSoundVolume);
        }

        // Trigger collision event
        onObstacleCollision.Invoke(obstacle);
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        onInvincibilityStart.Invoke();
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        onInvincibilityEnd.Invoke();
    }

    private void ApplyKnockback(Vector2 direction)
    {
        if (rb != null)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;
            knockbackDirection = direction;
            
            // Store current velocity
            Vector2 currentVelocity = rb.linearVelocity;
            
            // Apply knockback force
            rb.linearVelocity = direction * knockbackForce;
            
            // Optional: Preserve some of the original velocity
            rb.linearVelocity += currentVelocity * 0.2f;
        }
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
        if (rb != null)
        {
            // Optional: Reset or adjust velocity when knockback ends
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
        }
    }

    // Public methods for external control
    public bool IsInvincible() => isInvincible;
    public bool IsKnockedBack() => isKnockedBack;
    public void ForceEndInvincibility() => EndInvincibility();
} 