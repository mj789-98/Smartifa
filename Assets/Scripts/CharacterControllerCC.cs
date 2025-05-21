using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterControllerCC : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Initial movement speed")]
    public float initialSpeed = 5f;
    
    [Tooltip("How quickly the speed increases over time")]
    public float acceleration = 0.1f;
    
    [Tooltip("Maximum speed the character can reach")]
    public float maxSpeed = 20f;

    [Header("Jump Settings")]
    [Tooltip("Force applied when jumping")]
    public float jumpForce = 12f;
    
    [Tooltip("Time window for double jump (in seconds)")]
    public float doubleJumpWindow = 0.5f;
    
    [Tooltip("Layer mask for ground detection")]
    public LayerMask groundLayer;
    
    [Tooltip("Distance to check for ground")]
    public float groundCheckDistance = 0.5f;
    
    [Tooltip("Whether to use keyboard input for testing (spacebar)")]
    public bool useKeyboardInput = true;

    private float currentSpeed;
    private Rigidbody2D rb;
    private bool canDoubleJump = false;
    private bool isGrounded = false;
    private float lastJumpTime;
    private BoxCollider2D boxCollider;
    private bool jumpWasPressed = false;

    private void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        if (rb == null || boxCollider == null)
        {
            Debug.LogError("Required components missing from character!");
            enabled = false;
            return;
        }

        // Set initial speed
        currentSpeed = initialSpeed;
        
        // Configure Rigidbody2D for optimal 2D movement
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Set up layer mask if not set
        if (groundLayer.value == 0)
        {
            Debug.LogWarning("Ground layer not set! Trying to find a layer named 'Ground'");
            int groundLayerIndex = LayerMask.NameToLayer("Ground");
            if (groundLayerIndex != -1)
            {
                groundLayer = 1 << groundLayerIndex;
                Debug.Log($"Ground layer found and set to layer index: {groundLayerIndex}, Mask: {groundLayer.value}");
            }
            else
            {
                // Try to find any layer with objects that might be ground
                int defaultLayer = LayerMask.NameToLayer("Default");
                groundLayer = 1 << defaultLayer;
                Debug.LogWarning($"No 'Ground' layer found, using 'Default' layer ({defaultLayer}). Create a 'Ground' layer and assign objects to it.");
            }
        }

        // Position player above the ground at start
        transform.position = new Vector3(0, 1.0f, 0);
        
        Debug.Log("Character Controller initialized successfully. Ground layer mask: " + groundLayer.value);
    }

    private void Update()
    {
        // Check if we're grounded
        CheckGrounded();
        
        // Handle jump input - both keyboard and touch
        HandleJumpInput();
    }
    
    private void HandleJumpInput()
    {
        bool jumpPressed = false;
        
        // Handle keyboard input (for testing in editor)
        if (useKeyboardInput && Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }
        
        // Handle touch input (for mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                jumpPressed = true;
                Debug.Log("Touch detected for jump");
            }
        }
        
        // Process jump if either input was detected
        if (jumpPressed && !jumpWasPressed)
        {
            jumpWasPressed = true;
            PerformJump();
        }
        else if (!Input.GetKey(KeyCode.Space) && Input.touchCount == 0)
        {
            jumpWasPressed = false;
        }
    }

    private void FixedUpdate()
    {
        // Update speed (clamped to maxSpeed)
        currentSpeed = Mathf.Min(currentSpeed + (acceleration * Time.fixedDeltaTime), maxSpeed);
        
        // Move character forward
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void CheckGrounded()
    {
        if (boxCollider == null) return;
        
        // Check with multiple methods for more reliable detection
        bool hitDetected = CheckRaycast() || CheckOverlap();
        
        // Update grounded state
        bool wasGrounded = isGrounded;
        isGrounded = hitDetected;

        // Reset double jump when landing
        if (isGrounded && !wasGrounded)
        {
            canDoubleJump = false;
            Debug.Log("Landed on ground");
        }
        else if (!isGrounded && wasGrounded)
        {
            Debug.Log("Left the ground");
        }
    }
    
    private bool CheckRaycast()
    {
        // Get the bounds of the collider
        Bounds bounds = boxCollider.bounds;
        float width = bounds.size.x * 0.8f;
        
        // Cast rays from left, center, and right of the collider
        Vector2 leftOrigin = new Vector2(bounds.min.x + (width * 0.1f), bounds.min.y + 0.05f);
        Vector2 centerOrigin = new Vector2(bounds.center.x, bounds.min.y + 0.05f);
        Vector2 rightOrigin = new Vector2(bounds.max.x - (width * 0.1f), bounds.min.y + 0.05f);
        
        // Check each ray
        RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hitCenter = Physics2D.Raycast(centerOrigin, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.down, groundCheckDistance, groundLayer);
        
        // Visualize rays
        Debug.DrawRay(leftOrigin, Vector2.down * groundCheckDistance, hitLeft.collider != null ? Color.green : Color.red);
        Debug.DrawRay(centerOrigin, Vector2.down * groundCheckDistance, hitCenter.collider != null ? Color.green : Color.red);
        Debug.DrawRay(rightOrigin, Vector2.down * groundCheckDistance, hitRight.collider != null ? Color.green : Color.red);
        
        // If any ray hits, we're grounded
        bool rayCastHit = hitLeft.collider != null || hitCenter.collider != null || hitRight.collider != null;
        
        if (rayCastHit)
        {
            Debug.Log("Ground detected by raycast");
        }
        
        return rayCastHit;
    }
    
    private bool CheckOverlap()
    {
        // Get the bounds of the collider
        Bounds bounds = boxCollider.bounds;
        
        // Create a box slightly below the character
        Vector2 boxCenter = new Vector2(bounds.center.x, bounds.min.y - 0.1f);
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.1f);
        
        // Check for overlaps with the ground
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, groundLayer);
        
        // Debug visualization
        Debug.DrawLine(
            new Vector3(boxCenter.x - boxSize.x/2, boxCenter.y - boxSize.y/2, 0),
            new Vector3(boxCenter.x + boxSize.x/2, boxCenter.y - boxSize.y/2, 0),
            hits.Length > 0 ? Color.green : Color.yellow
        );
        
        // If we find at least one collider, we're grounded
        bool overlapHit = hits.Length > 0;
        
        if (overlapHit)
        {
            Debug.Log($"Ground detected by overlap. Hit objects: {hits.Length}");
            foreach (var hit in hits)
            {
                Debug.Log($"Overlap hit: {hit.gameObject.name}, Layer: {hit.gameObject.layer}");
            }
        }
        
        return overlapHit;
    }

    private void PerformJump()
    {
        Debug.Log($"Jump attempt - isGrounded: {isGrounded}, canDoubleJump: {canDoubleJump}");
        
        if (isGrounded)
        {
            // First jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpTime = Time.time;
            canDoubleJump = true;
            Debug.Log("First jump performed!");
        }
        else if (canDoubleJump && Time.time - lastJumpTime <= doubleJumpWindow)
        {
            // Double jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.8f);
            canDoubleJump = false;
            Debug.Log("Double jump performed!");
        }
        else
        {
            Debug.Log($"Jump attempted but conditions not met. isGrounded: {isGrounded}, canDoubleJump: {canDoubleJump}");
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            // Get the bounds for visualization
            Bounds bounds = boxCollider.bounds;
            float width = bounds.size.x * 0.8f;
            
            // Draw the ground check rays
            Vector2 leftOrigin = new Vector2(bounds.min.x + (width * 0.1f), bounds.min.y + 0.05f);
            Vector2 centerOrigin = new Vector2(bounds.center.x, bounds.min.y + 0.05f);
            Vector2 rightOrigin = new Vector2(bounds.max.x - (width * 0.1f), bounds.min.y + 0.05f);
            
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(leftOrigin, Vector2.down * groundCheckDistance);
            Gizmos.DrawRay(centerOrigin, Vector2.down * groundCheckDistance);
            Gizmos.DrawRay(rightOrigin, Vector2.down * groundCheckDistance);
            
            // Draw the overlap box
            Vector2 boxCenter = new Vector2(bounds.center.x, bounds.min.y - 0.1f);
            Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.1f);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
} 