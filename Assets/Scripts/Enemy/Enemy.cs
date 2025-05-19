using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health;

    [Header("Audio")]
    public AudioClip deathClip;      // ← assign in Inspector
    public AudioClip explodeClip;    // ← assign in Inspector for explosion sound
    public AudioClip hurtClip;
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
    private Vector3 _lastExplosionOrigin;

    private Vector3 _explosionOrigin => player != null ? player.position : transform.position;

    private AudioSource _audioSource;
    private bool _isDying = false;

    [Header("AI Settings")]
    public bool isRanged;
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsplayer;
    //Patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float waitTime;
    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    //States
    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    [Header("Melee Settings")]
    public int attackDamage = 20;
    public float damageRadius = 1f;

     [Header("Animation")]
    private Animator _animator;

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

    IEnumerator DeathSequence()
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

        Destroy(gibbed, 2f);
        Destroy(gameObject, 3f);
    }


    void SetRigidbodyState(bool isKinematic)
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

    void SetColliderState(bool enabled)
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

    void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    void Chase()
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

    void Attack()
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

    private void DealDamage()
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
            // Ranged attack logic (e.g. shooting a projectile)
            // Implement your ranged attack logic here
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        _animator.SetBool("Attacking", false);
    }

    private void Update()
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