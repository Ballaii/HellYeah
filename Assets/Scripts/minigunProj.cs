using UnityEngine;

/// <summary>
/// Handles damage, impact effects, and collision logic for enemy projectiles
/// </summary>
public class minigunProj : MonoBehaviour
{
    public int damage = 15;
    private bool hasHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        hasHit = true;

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
