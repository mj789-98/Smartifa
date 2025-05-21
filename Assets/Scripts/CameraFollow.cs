using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("The target to follow (usually the player)")]
    public Transform target;
    
    [Tooltip("How fast the camera follows the target")]
    public float smoothSpeed = 5f;
    
    [Tooltip("Offset from the target position")]
    public Vector3 offset = new Vector3(3f, 2f, -10f);

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to CameraFollow!");
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move towards that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Update camera position
        transform.position = smoothedPosition;
    }
} 