using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float speed = 25f;
    public float lifetime = 5f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public ParticleSystem explosionEffect;
    public AudioClip explosionSound;
    public int damage = 100;
    public string enemyTag = "Enemy";

    [Tooltip("Contrail particle system prefab.")]
    public GameObject contrailPrefab;

    private GameObject contrailInstance;


    void Start()
    {
        if (contrailPrefab != null)
        {
        contrailInstance = Instantiate(contrailPrefab, transform.position, Quaternion.identity, transform);
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rocket does not have a Rigidbody.");
        }

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (contrailInstance != null)
        {
        contrailInstance.transform.parent = null; // Detach
        ParticleSystem ps = contrailInstance.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(); // Stop emitting
            Destroy(contrailInstance, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(contrailInstance, 2f); // Fallback
        }
        }

        // Visual & Sound
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Apply explosion to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            GameObject enemy = nearby.gameObject;

            // Apply physics force
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Damage enemies
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyHealth = enemy.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }

            // Moveable furniture
            else if (enemy.CompareTag("Moveable"))
            {
                MoveableFurniture furniture = enemy.GetComponent<MoveableFurniture>();
                if (furniture != null)
                {
                    furniture.ApplyExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

            // Trigger music
            else if (enemy.CompareTag("MusicManager"))
            {
                MusicStarter musicManager = enemy.GetComponent<MusicStarter>();
                if (musicManager != null)
                {
                    musicManager.PlayMusicCoroutine();
                }
            }
            if (enemy.CompareTag("Barrel"))
            {
                BarrelScript barrel = enemy.GetComponent<BarrelScript>();
                if (barrel != null)
                {
                    barrel.Explode();
                }
            }
            if (enemy.CompareTag("Player"))
            {
                PlayerHealth playerHealth = enemy.GetComponent<PlayerHealth>();
                CPMPlayer controller = enemy.GetComponent<CPMPlayer>();
                if (controller != null)
                {
                    controller.RocketJump(transform.position, 7.5f);
                }
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
            }
        }

        // Destroy rocket
        Destroy(gameObject);
    }
}
