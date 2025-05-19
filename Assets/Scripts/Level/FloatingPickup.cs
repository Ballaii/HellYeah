using UnityEngine;

/// <summary>
/// Makes a GameObject float up and down in place.
/// Attach this to any pickup prefab to add a bobbing effect.
/// </summary>
public class FloatingPickup : MonoBehaviour
{
    [Header("Floating Settings")]
    [Tooltip("Maximum vertical displacement.")]
    public float amplitude = 0.25f;  // how far up and down

    [Tooltip("Speed of the bobbing motion.")]
    public float speed = 2f;        // how fast it moves

    private Vector3 startPos;

    void Start()
    {
        // Record the initial position
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate new Y position
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        float newX = startPos.x + Mathf.Cos(Time.time * speed) * amplitude;
        transform.position = new Vector3(newX, newY, startPos.z);
    }
}
