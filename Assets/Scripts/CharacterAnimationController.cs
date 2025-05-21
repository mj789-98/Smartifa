using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    // Animation State Names - these should match your animation clip names
    public static class AnimationStates
    {
        public const string Idle = "Idle";
        public const string Run = "Run";
        public const string JumpStart = "JumpStart";
        public const string JumpAir = "JumpAir";
        public const string DoubleJump = "DoubleJump";
        public const string Fall = "Fall";
        public const string Land = "Land";
        public const string Hit = "Hit";
    }

    // Animation Parameters - these should match your Animator parameters
    public static class AnimParams
    {
        public const string IsGrounded = "isGrounded";
        public const string VelocityY = "velocityY";
        public const string IsJumping = "isJumping";
        public const string IsDoubleJumping = "isDoubleJumping";
        public const string IsHit = "isHit";
        public const string Speed = "speed";
    }

    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animation Settings")]
    [SerializeField] private float landingThreshold = -0.1f;  // Velocity threshold to trigger landing
    [SerializeField] private float fallThreshold = -2f;       // Velocity threshold to trigger falling

    private bool isGrounded;
    private bool wasGrounded;
    private bool isJumping;
    private bool isDoubleJumping;

    private void Awake()
    {
        // Get components if not assigned
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        // Update ground state
        wasGrounded = isGrounded;
        isGrounded = CheckGrounded(); // You'll need to implement this based on your ground detection method

        // Update animator parameters
        animator.SetBool(AnimParams.IsGrounded, isGrounded);
        animator.SetFloat(AnimParams.VelocityY, rb.linearVelocity.y);
        animator.SetFloat(AnimParams.Speed, Mathf.Abs(rb.linearVelocity.x));

        // Handle landing animation
        if (!wasGrounded && isGrounded && rb.linearVelocity.y <= landingThreshold)
        {
            animator.Play(AnimationStates.Land);
        }

        // Reset jump flags when grounded
        if (isGrounded)
        {
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    // Call this when the player initiates a jump
    public void TriggerJump()
    {
        if (isGrounded)
        {
            isJumping = true;
            animator.SetTrigger(AnimParams.IsJumping);
            animator.Play(AnimationStates.JumpStart);
        }
        else if (isJumping && !isDoubleJumping)
        {
            isDoubleJumping = true;
            animator.SetTrigger(AnimParams.IsDoubleJumping);
            animator.Play(AnimationStates.DoubleJump);
        }
    }

    // Call this when the player gets hit
    public void TriggerHit()
    {
        animator.SetTrigger(AnimParams.IsHit);
    }

    // Implement this based on your ground detection method
    private bool CheckGrounded()
    {
        // Example implementation using raycast
        float rayLength = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            rayLength,
            LayerMask.GetMask("Ground")
        );
        return hit.collider != null;
    }

    // Animation event handlers (called from animation clips)
    public void OnJumpStartComplete()
    {
        animator.Play(AnimationStates.JumpAir);
    }

    public void OnLandComplete()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            animator.Play(AnimationStates.Run);
        }
        else
        {
            animator.Play(AnimationStates.Idle);
        }
    }
} 