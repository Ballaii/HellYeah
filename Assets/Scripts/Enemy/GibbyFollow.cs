using UnityEngine;

/// <summary>
/// Attach this script to the GameObject you want to move.
/// It will match the position of the specified target GameObject every frame.
/// </summary>
public class FollowPosition : MonoBehaviour
{
    [Tooltip("The target GameObject whose position this object will match.")]
    public Transform target;

    // If you want an optional offset, you can uncomment the following line:
    // public Vector3 offset;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("[FollowPosition] No target set for " + name + ". Please assign a target in the inspector.");
            return;
        }

        // Match position exactly
        transform.position = target.position;

    }
}
