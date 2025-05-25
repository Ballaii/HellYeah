using UnityEngine;

/// <summary>
/// Handles damage, impact effects, and collision logic for enemy projectiles
/// </summary>
public class minigunProj : MonoBehaviour
{
     public float speed = 20f;
    public int damage = 15;
    public float lifetime = 5f;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void Start()
    {
        _rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Try to get the player component from what we hit
        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        // Optional: add explosion or impact effect here

        Destroy(gameObject);
    }
}
