using UnityEngine;

/// <summary>
/// Handles damage, impact effects, and collision logic for shotgun pellets
/// </summary>
public class ShotgunPellet : MonoBehaviour
{
    public int damage=25;
    private bool targetHit;
    public int multiplier = 1;

    private void OnCollisionEnter(Collision collision)
    {
        multiplier = CigController.CurrentDamageMultiplier;
        if (!targetHit)
        {
            targetHit = true;

            Enemy enemy = collision.collider.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage * multiplier, transform.position);

                Destroy(gameObject);
            }
        }
    }
}