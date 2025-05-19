using UnityEngine;

public class SwordController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator; // Animator controlling the sword swing animation
    public AudioSource audioSource; // AudioSource for playing sounds
    public AudioClip swingSound; // Sound played when swinging the sword
    public AudioClip error;

    [Header("Attack Settings")]
    public float attackCooldown = 0.7f; // Time between attacks
    private bool isAttacking = false;
    bool targetHit = false;

    public int damage = 100;
    public int multiplier = 1;

    public Collider hitbox;
    private Coroutine attackCoroutine;


    void Update()
    {
        if (PauseMenu.paused) return;
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
        attackCoroutine = StartCoroutine(PerformAttack());
        }

    }
    
    private void OnDisable()
{
    if (attackCoroutine != null)
    {
        StopCoroutine(attackCoroutine);
        attackCoroutine = null;
    }

    isAttacking = false;
    targetHit = false;

    if (hitbox != null)
    {
        hitbox.enabled = false;
    }

    if (animator != null)
    {
        animator.Play("idle");
    }
}


    private System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;

        hitbox.GetComponent<Collider>().enabled = true;

        // Play swing animation
        if (animator != null)
            GetComponent<Animator>().Play("Swing");
        else
        {
            Debug.LogError("Animator is not assigned.");
            audioSource.PlayOneShot(error);
        }

        // Play swing sound
        if (swingSound != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(attackCooldown);

        GetComponent<Animator>().Play("idle");

        hitbox.GetComponent<Collider>().enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!targetHit && other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            targetHit = true;
            
            Debug.Log("hit enemy");
            enemy.TakeDamage(damage * multiplier, hitbox.transform.position);
            //Destroy(gameObject);
        }
    }


}
