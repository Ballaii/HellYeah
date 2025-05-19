using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource), typeof(NavMeshAgent))]
public class TRexBoss : MonoBehaviour
{
    public enum BossState { Idle, Taunt, TailAttack, GunAttack, Recover, Dying, Dead }
    private BossState _state = BossState.Idle;

    [Header("Boss Stats")]
    public int maxHealth = 2000;
    private int currentHealth;

    [Header("Detection & Movement")]
    public Transform player;
    public float chaseRange = 20f;
    public float stopRange = 15f;
    public float idleMoveSpeed = 3.5f;
    public float combatMoveSpeed = 5f;
    private NavMeshAgent agent;

    [Header("Audio Clips")]
    public AudioClip roarClip;
    public AudioClip deathClip;
    public AudioClip explodeClip;
    public AudioClip tailAttackClip;
    public AudioClip gunAttackClip;
    public float roarVolume = 1f;
    public float deathVolume = 0.7f;
    public float explodeVolume = 0.8f;
    public float tailAttackVolume = 0.6f;
    public float gunAttackVolume = 0.5f;

    [Header("Tail Attack")]
    public Collider tailCollider;
    public float tailDamage = 50f;
    public float tailWindup = 0.5f;
    public float tailDuration = 1f;
    public float tailRecovery = 1f;
    public float tailAttackCooldown = 3f;
    private float lastTailAttackTime;

    [Header("Gun Attack")]
    public Transform[] gunMuzzles;
    public GameObject projectilePrefab;
    public float projectileSpeed = 50f;
    public float fireRate = 0.1f;
    public int burstCount = 10;
    public float gunRecovery = 2f;
    public float gunAttackCooldown = 5f;
    private float lastGunAttackTime;

    [Header("Taunt")]
    public float tauntDuration = 2f;
    public float tauntCooldown = 8f;
    private float _lastTauntTime;

    [Header("Death Effects")]
    public GameObject aliveModel;
    public GameObject gibModel;
    public ParticleSystem gibParticleEffect;
    public float minRagdollDuration = 1f;
    public float maxRagdollDuration = 2f;
    public Transform explosionOrigin;
    public float explosionForce = 200f;
    public float explosionRadius = 5f;
    public float explosionUpward = 0.5f;

    [Header("UI Elements")]
    public Slider healthBar;
    public GameObject healthBarCanvas;

    [Header("Debug")]
    public bool debugMode = false;

    private Animator animator;
    private AudioSource audioSource;
    private float gunEnableThreshold;
    private float defaultFixedDelta;
    private bool isPlayerInRange = false;

    void Awake()
    {
        // Initialize components
        currentHealth = maxHealth;
        gunEnableThreshold = maxHealth * 0.5f;
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("TRexBoss: Animator component not found!");
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) Debug.LogError("TRexBoss: AudioSource component not found!");
        else audioSource.playOnAwake = false;
        
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("TRexBoss: NavMeshAgent component not found!");

        // Initialize UI
        if (healthBar)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        
        // Initialize colliders and models
        if (tailCollider) tailCollider.enabled = false;
        else Debug.LogWarning("TRexBoss: Tail collider not assigned!");
        
        if (gibModel) 
        { 
            gibModel.SetActive(false); 
            var rigidbodies = gibModel.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
            }
        }
        else Debug.LogWarning("TRexBoss: Gib model not assigned!");
        
        if (aliveModel) aliveModel.SetActive(true);
        else Debug.LogWarning("TRexBoss: Alive model not assigned!");

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj) player = playerObj.transform;
            else Debug.LogError("TRexBoss: Player not found! Please tag your player with 'Player' tag.");
        }

        defaultFixedDelta = Time.fixedDeltaTime;
        
        // Initialize attack cooldowns
        lastTailAttackTime = -tailAttackCooldown; // Allow immediate attack at start
        lastGunAttackTime = -gunAttackCooldown;
        _lastTauntTime = -tauntCooldown;
    }

    void OnEnable()
    {
        // Reset state when enabled
        _state = BossState.Idle;
        if (agent) agent.isStopped = false;
    }

    void Update()
    {
        if (_state == BossState.Dead || _state == BossState.Dying) return;
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distToPlayer <= chaseRange;

        // Update health bar position if needed
        if (healthBarCanvas)
        {
            healthBarCanvas.SetActive(isPlayerInRange);
        }

        // Movement animation control
        bool isMoving = agent && agent.velocity.magnitude > 0.1f && _state == BossState.Idle;
        if (animator) animator.SetBool("Moving", isMoving);

        // Debug visualization
        if (debugMode)
        {
            Debug.DrawLine(transform.position, player.position, isPlayerInRange ? Color.red : Color.green);
            Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
        }

        // State transitions based on current state
        switch (_state)
        {
            case BossState.Idle:
                UpdateIdleState(distToPlayer);
                break;
            
            // Other states are managed by coroutines
        }
    }

    private void UpdateIdleState(float distToPlayer)
    {
        // Set appropriate movement speed
        if (agent && isPlayerInRange)
        {
            agent.speed = combatMoveSpeed;
        }
        else if (agent)
        {
            agent.speed = idleMoveSpeed;
        }

        // Choose next action based on conditions
        if (isPlayerInRange)
        {
            // Can perform a taunt
            if (Time.time - _lastTauntTime > tauntCooldown)
            {
                TransitionTo(BossState.Taunt);
                return;
            }
            
            // Close enough for tail attack
            if (distToPlayer <= stopRange && Time.time - lastTailAttackTime > tailAttackCooldown)
            {
                TransitionTo(BossState.TailAttack);
                return;
            }
            
            // Health low enough for gun attack
            if (currentHealth <= gunEnableThreshold && Time.time - lastGunAttackTime > gunAttackCooldown)
            {
                TransitionTo(BossState.GunAttack);
                return;
            }

            // If no special action, keep chasing/facing player
            FaceOrChase(distToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    void TransitionTo(BossState next)
    {
        if (debugMode) Debug.Log($"TRexBoss state change: {_state} -> {next}");
        
        _state = next;
        StopAllCoroutines();
        
        if (agent) agent.isStopped = true;
        if (animator)
        {
            animator.SetBool("Moving", false);
            animator.SetBool("Attacking", false);
        }

        switch (next)
        {
            case BossState.Taunt: 
                StartCoroutine(DoTaunt()); 
                break;
            case BossState.TailAttack: 
                StartCoroutine(DoTailAttack()); 
                break;
            case BossState.GunAttack: 
                StartCoroutine(DoGunAttack()); 
                break;
            case BossState.Dying: 
                StartCoroutine(DoDeathSequence()); 
                break;
            case BossState.Idle: 
                if (agent) agent.isStopped = false; 
                break;
        }
    }

    private void FaceOrChase(float distToPlayer)
    {
        if (player == null) return;

        // Always face player
        Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPosition);
        
        // Chase if player is beyond stop range
        if (agent)
        {
            if (distToPlayer > stopRange)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    private IEnumerator DoTaunt()
    {
        if (animator) animator.SetTrigger("Roar");
        if (audioSource && roarClip) audioSource.PlayOneShot(roarClip, roarVolume);
        _lastTauntTime = Time.time;
        
        yield return new WaitForSeconds(tauntDuration);
        
        TransitionTo(BossState.Idle);
    }

    private IEnumerator DoTailAttack()
    {
        lastTailAttackTime = Time.time;
        
        if (animator)
        {
            animator.SetBool("Attacking", true);
            animator.SetTrigger("TailSwing");
        }
        
        if (audioSource && tailAttackClip) audioSource.PlayOneShot(tailAttackClip, tailAttackVolume);
        
        // Wind-up phase
        yield return new WaitForSeconds(tailWindup);
        
        // Active damage phase
        if (tailCollider) tailCollider.enabled = true;
        yield return new WaitForSeconds(tailDuration);
        if (tailCollider) tailCollider.enabled = false;
        
        // Recovery phase
        yield return new WaitForSeconds(tailRecovery);
        
        if (animator) animator.SetBool("Attacking", false);
        TransitionTo(BossState.Idle);
    }

    private IEnumerator DoGunAttack()
    {
        // Safety check
        if (currentHealth > gunEnableThreshold)
        { 
            TransitionTo(BossState.Idle); 
            yield break; 
        }
        
        lastGunAttackTime = Time.time;
        
        if (animator)
        {
            animator.SetBool("Attacking", true);
            animator.SetTrigger("GunFire");
        }
        
        if (audioSource && gunAttackClip) audioSource.PlayOneShot(gunAttackClip, gunAttackVolume);
        
        // Make sure we're facing the player for the attack
        if (player != null)
        {
            Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookPosition);
        }
        
        // Fire projectiles
        for (int i = 0; i < burstCount; i++)
        {
            foreach (var muzzle in gunMuzzles)
            {
                if (muzzle && projectilePrefab)
                {
                    GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
                    Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
                    
                    if (projectileRb)
                    {
                        projectileRb.linearVelocity = muzzle.forward * projectileSpeed;
                    }
                    else
                    {
                        // If no rigidbody, try to find a projectile component
                        var projectileComponent = projectile.GetComponent<Projectile>();
                        if (projectileComponent)
                        {
                            projectileComponent.Fire(muzzle.forward * projectileSpeed);
                        }
                    }
                    
                    // Clean up projectile after some time
                    Destroy(projectile, 5f);
                }
            }
            yield return new WaitForSeconds(fireRate);
        }
        
        // Recovery phase
        yield return new WaitForSeconds(gunRecovery);
        
        if (animator) animator.SetBool("Attacking", false);
        TransitionTo(BossState.Idle);
    }

    public void TakeDamage(int damage)
    {
        if (_state == BossState.Dead || _state == BossState.Dying) return;
        
        // Apply damage
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        
        // Update UI
        if (healthBar) healthBar.value = currentHealth;
        
        // Flash the model to indicate damage
        StartCoroutine(FlashDamage());
        
        // Check for death
        if (currentHealth <= 0)
        {
            TransitionTo(BossState.Dying);
        }
        // Maybe trigger a taunt on big hits
        else if (Random.value < 0.2f && _state == BossState.Idle && Time.time - _lastTauntTime > tauntCooldown / 2)
        {
            TransitionTo(BossState.Taunt);
        }
    }

    private IEnumerator FlashDamage()
    {
        // Flash the material to red to indicate damage
        if (aliveModel)
        {
            Renderer[] renderers = aliveModel.GetComponentsInChildren<Renderer>();
            Color originalColor = Color.white;
            
            foreach (Renderer r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                {
                    originalColor = r.material.color;
                    r.material.color = Color.red;
                }
            }
            
            yield return new WaitForSeconds(0.1f);
            
            foreach (Renderer r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                {
                    r.material.color = originalColor;
                }
            }
        }
    }

    private IEnumerator DoDeathSequence()
    {
        // Disable the agent and colliders
        if (agent) agent.enabled = false;
        
        // Play death animation and sound
        if (animator) animator.SetTrigger("Die");
        if (audioSource && deathClip) audioSource.PlayOneShot(deathClip, deathVolume);

        // Ragdoll phase - enable physics on the alive model
        if (aliveModel)
        {
            Rigidbody[] rigidbodies = aliveModel.GetComponentsInChildren<Rigidbody>();
            Collider[] colliders = aliveModel.GetComponentsInChildren<Collider>();
            
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
            }
            
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
        }

        // Wait for the ragdoll to settle
        float ragdollTime = Random.Range(minRagdollDuration, maxRagdollDuration);
        yield return new WaitForSeconds(ragdollTime);

        // Optional: Slow-motion effect
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = defaultFixedDelta * Time.timeScale;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDelta;

        // Gib explosion effect
        if (gibParticleEffect)
        {
            ParticleSystem gibEffect = Instantiate(gibParticleEffect, transform.position, Quaternion.identity);
            gibEffect.Play();
            Destroy(gibEffect.gameObject, gibEffect.main.duration + 1f);
        }
        
        if (audioSource && explodeClip) audioSource.PlayOneShot(explodeClip, explodeVolume);

        // Switch to gib model
        if (aliveModel) aliveModel.SetActive(false);
        if (gibModel) 
        {
            gibModel.SetActive(true);
            
            // Apply explosion force to all gib parts
            Rigidbody[] gibRigidbodies = gibModel.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in gibRigidbodies)
            {
                rb.isKinematic = false;
                if (explosionOrigin)
                {
                    rb.AddExplosionForce(explosionForce, explosionOrigin.position, explosionRadius, explosionUpward, ForceMode.Impulse);
                }
            }
        }

        // Hide health bar if applicable
        if (healthBarCanvas) healthBarCanvas.SetActive(false);

        // Wait before destroying the game object
        yield return new WaitForSeconds(5f); // Give more time for gibs to be visible
        
        _state = BossState.Dead;
        
        // Either destroy or disable based on your game's design
        gameObject.SetActive(false); // Sometimes better than destroying for potential respawn
        //Destroy(gameObject);
    }

    private void Patrol()
    {
        if (!agent) return;
        
        agent.isStopped = false;
        
        // For a more complex patrol behavior, you could implement waypoints
        // For now we'll just have it move randomly when the player is not in range
        if (agent.remainingDistance < 0.5f || !agent.hasPath)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only deal damage during tail attack state and only to player
        if (_state == BossState.TailAttack && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)tailDamage);
            }
            else
            {
                // Fallback if the player doesn't have the exact component
                var damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)tailDamage);
                }
                else
                {
                    Debug.LogWarning("Player has no health component that can be damaged!");
                }
            }
        }
    }

    // For optional projectile behavior if not using rigidbody
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }

    // Simple projectile class that could be used by the projectiles
    [System.Serializable]
    public class Projectile : MonoBehaviour
    {
        public float damage = 10f;
        public float lifetime = 5f;
        private Vector3 velocity;
        
        public void Fire(Vector3 velocity)
        {
            this.velocity = velocity;
            Destroy(gameObject, lifetime);
        }
        
        void Update()
        {
            if (velocity != Vector3.zero)
            {
                transform.position += velocity * Time.deltaTime;
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth)
                {
                    playerHealth.TakeDamage((int)damage);
                }
                Destroy(gameObject);
            }
            else if (!other.CompareTag("Boss") && !other.CompareTag("Projectile"))
            {
                // Hit something else (not boss or another projectile)
                Destroy(gameObject);
            }
        }
    }

    // Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
        
        if (tailCollider)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(tailCollider.bounds.center, tailCollider.bounds.size);
        }
    }
}