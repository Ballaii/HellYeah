using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health;

    [Header("Audio")]
    public AudioClip deathClip;      // ← assign in Inspector
    public AudioClip explodeClip;    // ← assign in Inspector for explosion sound
    public AudioClip hurtClip;
    public AudioClip attackClip; // ← assign in Inspector for attack sound
    public AudioSource source;
    [Range(0f, 1f)] public float deathVolume = 0.7f;
    [Range(0f, 1f)] public float explodeVolume = 0.8f;

    [Header("Death Effects")]
    public float minRagdollDuration = 0.2f;
    public float maxRagdollDuration = 0.8f;
    public GameObject alive;
    public GameObject gibbed;
    public Collider hitbox;
    public ParticleSystem gibParticleEffect;

    [Header("Blast Settings")]
    public Transform player;           // assign in Inspector
    public float explosionForce = 500f;
    public float explosionRadius = 3f;
    public Vector3 _lastExplosionOrigin;

    public Vector3 _explosionOrigin => player != null ? player.position : transform.position;

    public AudioSource _audioSource;
    public bool _isDying = false;

    [Header("AI Settings")]
    public bool isRanged;
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsplayer;
    //Patrol
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public float waitTime;
    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Attack Settings")]
    public int attackDamage = 20;
    public float damageRadius = 1f;
    public GameObject projectile;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 200f;
    public float projectileLifetime = 4f;
    public bool canShoot = true;



     [Header("Animation")]
    public Animator _animator;
    public ComboManager comboManager;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (source == null)
            source = GetComponent<AudioSource>();

        _audioSource = source;
        _audioSource.playOnAwake = false;
    }

    void Start()
    {
        SetRigidbodyState(true);
        SetColliderState(false);
        gibbed.SetActive(false);
        gibbed.GetComponent<Rigidbody>().isKinematic = true;
        gibbed.GetComponent<Rigidbody>().isKinematic = true;
        gibbed.GetComponent<Collider>().enabled = false;
        alive.GetComponent<Collider>().enabled = true;
        alive.GetComponent<Rigidbody>().isKinematic = true;
    }
    public void TakeDamage(int damage, Vector3 explosionOrigin)
    {
        if (_isDying) return;
        _lastExplosionOrigin = explosionOrigin;

        Debug.Log("Enemy took " + damage + " damage.");
        _audioSource.PlayOneShot(hurtClip);
        health -= damage;
        if (health <= 0)
            StartCoroutine(DeathSequence());
    }
    
    public void TakeDamage(int damage)
    {
        if (_isDying) return;
        _lastExplosionOrigin = _explosionOrigin;
        Debug.Log("Enemy took " + damage + " damage.");
        _audioSource.PlayOneShot(hurtClip);
        health -= damage;
        if (health <= 0)
            StartCoroutine(DeathSequence());
    }

    public IEnumerator DeathSequence()
    {
        _isDying = true;
        if (hitbox != null) hitbox.enabled = false;
        if (deathClip != null) _audioSource.PlayOneShot(deathClip, deathVolume);

        // 1) Show ragdoll
        alive.SetActive(true);
        gibbed.SetActive(false);

        SetRigidbodyState(false);
        SetColliderState(true);

        // — HERE: blast all child rigidbodies outward! —
        Vector3 blastCenter = _lastExplosionOrigin;
        // or: Vector3 blastCenter = _explosionOrigin;
        float radius = explosionRadius;
        float force = explosionForce;
        float tunedForce = 20f;
        float tunedRadius = 5f;
        float downBias = -1f;


        foreach (Rigidbody rb in alive.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.AddExplosionForce(
            tunedForce,
            blastCenter,
            tunedRadius,
            downBias,
            ForceMode.Impulse
        );
        }

        float randomDuration = Random.Range(minRagdollDuration, maxRagdollDuration);
        yield return new WaitForSeconds(randomDuration);

        // 2) Gib explosion…
        if (gibParticleEffect != null)
            Instantiate(gibParticleEffect, transform.position, Quaternion.identity).Play();

        if (explodeClip != null) _audioSource.PlayOneShot(explodeClip, explodeVolume);

        alive.SetActive(false);
        gibbed.SetActive(true);


        foreach (Rigidbody rb in gibbed.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        WaveManager.enemiesDead++;
        comboManager.RegisterKill();

        Destroy(gibbed, 2f);
        Destroy(gameObject, 3f);
    }


    public void SetRigidbodyState(bool isKinematic)
    {
        Rigidbody[] rigidbodies = alive.GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = isKinematic;
        }

        Rigidbody mainRigidbody = alive.GetComponent<Rigidbody>();
        if (mainRigidbody != null)
            mainRigidbody.isKinematic = !isKinematic;
    }

    public void SetColliderState(bool enabled)
    {
        Collider[] colliders = alive.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = enabled;
        }

        Collider mainCollider = alive.GetComponent<Collider>();
        if (mainCollider != null)
            mainCollider.enabled = !enabled;
    }

    //AI

    public void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    public void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    public void Chase()
    {
        // build a target that has the same X we already are at,
        // but the player’s Z and Y (height) so we chase straight forward/backward only:
        Vector3 chaseTarget = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );
        //_animator.SetTrigger("Run");
        agent.SetDestination(chaseTarget);
    }

    public void Attack()
    {
        // we’re already at our own position, so just lock movement entirely:
        agent.SetDestination(transform.position);

        // same trick for rotation: only look “forward/backward” in Z,
        // leave our own X untouched so we don’t tip or sidestep:
        Vector3 lookTarget = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );
        transform.LookAt(lookTarget);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            _animator.SetBool("Attacking", true);
            //_animator.SetTrigger("Slash");

            Invoke(nameof(DealDamage), 0.3f);

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void DealDamage()
    {
        if (!isRanged)
        {
            // Detect player in a sphere around a “hit point” (e.g. in front of enemy)
            Vector3 hitPoint = transform.position + transform.forward * (attackRange * 0.5f);
            Collider[] hits = Physics.OverlapSphere(hitPoint, damageRadius, whatIsplayer);
            foreach (var hit in hits)
            {
                // Assuming the player has a component called “PlayerHealth”
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(attackDamage);
                }
            }
        }
        else
        {
            if (canShoot)
            {
                ShootProjectile();
            }
        }
    }

    public void ShootProjectile()
    {
        canShoot = false;
        _animator.SetBool("Attacking", true);
        Invoke(nameof(ResetCanShoot), timeBetweenAttacks);

        //from TREX//ShotgunController
        if (player != null)
        {
            Vector3 lookPosition = new Vector3(player.position.x, player.position.y, player.position.z);
            transform.LookAt(lookPosition);
        }

        if (projectileSpawnPoint == null || projectile == null)
            return;

        // 1) Compute the full 3D direction to the player
        Vector3 toPlayer = (player.position - projectileSpawnPoint.position).normalized;

        // 2) Create a rotation that looks along that vector
        Quaternion aimRot = Quaternion.LookRotation(toPlayer, Vector3.up);

        // 3) Spawn pellets
        for (int i = 0; i < 6; i++)
        {
            pellets.Add(Quaternion.identity);
            pellets[i] = Quaternion.Euler(0, Random.Range(-10f, 10f), 0) * aimRot; // Add some random spread
            //pellets[i] = Quaternion.RotateTowards(pellets[i], aimRot, 30);
            SpawnProjectile(pellets[i], toPlayer);
        }
        if (attackClip) _audioSource.PlayOneShot(attackClip);
        _animator.SetBool("Attacking", false);
    
    }
    List<Quaternion> pellets = new List<Quaternion>();

    private void SpawnProjectile(Quaternion aimRot, Vector3 toPlayer)
    {
        GameObject proj = Instantiate(projectile, projectileSpawnPoint.position, aimRot);
        if (proj != null)
        {
            // Get the Rigidbody component and set its velocity
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = toPlayer * projectileSpeed;
            }

            Destroy(proj, 5f);
        }
    }

      private object ResetCanShoot()
      {
            canShoot = true;
            return null;
      }

      public void ResetAttack()
    {
        alreadyAttacked = false;
        _animator.SetBool("Attacking", false);
    }

    public void Update()
    {
        float speed = agent.velocity.magnitude / agent.speed; // normalized [0..1]
        _animator.SetFloat("Speed", speed);

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsplayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsplayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();
    }
}