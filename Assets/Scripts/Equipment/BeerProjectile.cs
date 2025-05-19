using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BeerProjectile : MonoBehaviour
{
    [Header("Impact FX")]
    public GameObject glassShatterEffect;  // assign your GlassShatter_PS prefab here
    public AudioClip breakSound;           // optional glass-break SFX
    public float destroyDelay = 0.1f;      // so particles have time to spawn

    private bool hasBroken = false;

    void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple triggers
        if (hasBroken) return;
        hasBroken = true;

        // 1. Spawn glass-shatter particle effect
        if (glassShatterEffect != null)
        {
            Instantiate(
                glassShatterEffect,
                transform.position,
                Quaternion.LookRotation(collision.contacts[0].normal)
            );
        }

        // 2. Optionally play a glass-break sound
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // 3. Disable visuals / physics of the bottle itself
        var rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;

        // 4. Destroy the gameObject after a short delay
        Destroy(gameObject, destroyDelay);
        DestroyImmediate(glassShatterEffect, true);
    }
}
