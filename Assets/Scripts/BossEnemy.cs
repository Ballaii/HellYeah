// BossEnemy.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BossEnemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 200;
    
    [Header("Audio")]
    public AudioClip deathClip;
    public AudioClip explodeClip;
    public AudioClip hurtClip;
    public AudioClip tauntClip;
    [Range(0f,1f)] public float deathVolume = 0.7f;
    [Range(0f,1f)] public float explodeVolume = 0.8f;
    [Range(0f,1f)] public float hurtVolume = 1f;
    [Range(0f,1f)] public float tauntVolume = 0.7f;
    private AudioSource _audioSource;

    [Header("Models & Ragdoll")]
    public GameObject aliveModel;      // your animated hull
    public GameObject gibbedModel;     // your ragdoll pieces
    public Collider hitbox;            // main collider to disable on death
    public ParticleSystem deathVFX;
    public float minRagdollTime = 0.2f;
    public float maxRagdollTime = 0.8f;
    private Vector3 _lastExplosionOrigin;

    [Header("Nav & Senses")]
    public Transform player;
    public LayerMask groundMask, playerMask;
    public NavMeshAgent agent;
    public float sightRange = 15f;
    public float meleeRange = 2f;
    public float rangedRange = 12f;

    [Header("Patrol")]
    public float walkRadius = 10f;
    private Vector3 _walkPoint;
    private bool _walkPointSet;

    [Header("Melee Attack")]
    public float timeBetweenMelee = 2f;
    private bool _meleeOnCooldown;
    public int meleeDamage = 20;
    public float meleeRadius = 1f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float timeBetweenRanged = 3f;
    private bool _rangedOnCooldown;

    [Header("Taunt")]
    [Range(0f,1f)] public float tauntChance = 0.25f;
    public string tauntTrigger = "Taunt";
    private bool _taunting;

    [Header("Animation")]
    private Animator _animator;

    private bool _isDying = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        _animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        // Ensure clean ragdoll setup
        aliveModel.SetActive(true);
        gibbedModel.SetActive(false);
        hitbox.enabled = true;

        // Kinematic for alive, non-kinematic for gib on death
        SetRagdollState(aliveModel, true);
        SetRagdollState(gibbedModel, false);
    }

    void Update()
    {
        if (_isDying) return;

        bool inSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        bool inMelee = Physics.CheckSphere(transform.position, meleeRange, playerMask);
        bool inRanged = Physics.CheckSphere(transform.position, rangedRange, playerMask);

        float speedNorm = agent.velocity.magnitude / agent.speed;
        _animator.SetFloat("Speed", speedNorm);

        if (!inSight)
        {
            Patrol();
            TryTaunt();
        }
        else
        {
            if (inMelee) DoMelee();
            else if (inRanged) DoRanged();
            else Chase();
        }
    }

    #region Patrol & Taunt
    private void Patrol()
    {
        if (!_walkPointSet)
        {
            Vector3 rnd = new Vector3(
                Random.Range(-walkRadius, walkRadius),
                0,
                Random.Range(-walkRadius, walkRadius)
            );
            _walkPoint = transform.position + rnd;
            if (Physics.Raycast(_walkPoint, Vector3.down, 2f, groundMask))
                _walkPointSet = true;
        }

        if (_walkPointSet)
        {
            agent.SetDestination(_walkPoint);
            if (Vector3.Distance(transform.position, _walkPoint) < 1f)
                _walkPointSet = false;
        }
    }

    private void TryTaunt()
    {
        if (_taunting) return;
        if (Random.value < tauntChance)
            StartCoroutine(Taunt());
    }

    private IEnumerator Taunt()
    {
        _taunting = true;
        _animator.SetTrigger(tauntTrigger);
        _audioSource.PlayOneShot(tauntClip, tauntVolume);
        yield return new WaitForSeconds(2f);
        _taunting = false;
    }
    #endregion

    #region Chase & Attacks
    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void DoMelee()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!_meleeOnCooldown)
            StartCoroutine(MeleeAttack());
    }

    private IEnumerator MeleeAttack()
    {
        _meleeOnCooldown = true;
        _animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.3f); // sync damage
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * (meleeRange * 0.5f), meleeRadius, playerMask);
        foreach (var c in hits)
            c.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);

        yield return new WaitForSeconds(timeBetweenMelee);
        _meleeOnCooldown = false;
        _animator.SetBool("Attacking", false);
    }

    private void DoRanged()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!_rangedOnCooldown)
            StartCoroutine(RangedAttack());
    }

    private IEnumerator RangedAttack()
    {
        _rangedOnCooldown = true;
        _animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.5f); // prep time

        // Fire projectile
        if (projectilePrefab != null && firePoint != null)
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        yield return new WaitForSeconds(timeBetweenRanged);
        _rangedOnCooldown = false;
    }
    #endregion

    #region Damage & Death
    public void TakeDamage(int dmg, Vector3 explosionOrigin)
    {
        if (_isDying) return;
        _lastExplosionOrigin = explosionOrigin;
        _audioSource.PlayOneShot(hurtClip, hurtVolume);
        health -= dmg;
        if (health <= 0) StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        _isDying = true;
        hitbox.enabled = false;
        _audioSource.PlayOneShot(deathClip, deathVolume);

        // switch to ragdoll
        aliveModel.SetActive(false);
        gibbedModel.SetActive(true);
        SetRagdollState(gibbedModel, true);

        // blast pieces
        foreach (Rigidbody rb in gibbedModel.GetComponentsInChildren<Rigidbody>())
            rb.AddExplosionForce(500f, _lastExplosionOrigin, 5f, -1f, ForceMode.Impulse);

        deathVFX?.Play();
        yield return new WaitForSeconds(Random.Range(minRagdollTime, maxRagdollTime));

        _audioSource.PlayOneShot(explodeClip, explodeVolume);
        Destroy(gameObject, 2f);
    }
    #endregion

    private void SetRagdollState(GameObject root, bool active)
    {
        foreach (var rb in root.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = !active;
        foreach (var col in root.GetComponentsInChildren<Collider>())
            col.enabled = active;
    }

    // Debug Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangedRange);
    }
}
