using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    public GameObject Barrel;
    public GameObject ExplosionParticle;
    public float ExplosionForce = 500f;
    public float ExplosionRadius = 5f;

    public AudioClip ExplosionSound;
    private AudioSource audiosource;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true; // Ensure the collider is a trigger
        audiosource = GetComponent<AudioSource>();
        Barrel.SetActive(true);
        ExplosionParticle.SetActive(false); // Ensure the explosion particle is inactive initially
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Explode()
    {
        ExplosionParticle.SetActive(true);
        audiosource.PlayOneShot(ExplosionSound);
        Instantiate(ExplosionParticle, transform.position + Vector3.up, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (Collider enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    enemy.GetComponent<Enemy>().TakeDamage(100); // Assuming the enemy has a TakeDamage method
                }
            }
            else if (enemy.CompareTag("Player"))
            {
                enemy.GetComponent<PlayerHealth>().TakeDamage(30); // Assuming the player has a TakeDamage method
            }
            else if (enemy.CompareTag("Moveable"))
            {
                MoveableFurniture furniture = enemy.GetComponent<MoveableFurniture>();
                if (furniture != null)
                {
                    furniture.ApplyExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
                }
            }
            
            else if (enemy.CompareTag("MusicManager"))
            {
                MusicStarter musicManager = enemy.GetComponent<MusicStarter>();
                MoveableFurniture furniture = enemy.GetComponent<MoveableFurniture>();
                if (furniture != null)
                {
                    furniture.ApplyExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
                }
                if (musicManager != null)
                {
                    musicManager.PlayMusicCoroutine();
                }
            }
        }
        Barrel.SetActive(false); // Deactivate the barrel after explosion
        GetComponent<Collider>().enabled = false; // Disable the collider to prevent further interactions
        Destroy(gameObject, 2f); // Destroy the barrel after 2 seconds
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Explode();
        }
    }

      private void OnDrawGizmos()
      {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
      }
}
