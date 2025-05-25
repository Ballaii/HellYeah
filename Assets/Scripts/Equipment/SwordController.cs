using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip swingSound;
    public AudioClip error;

    [Header("Attack Settings")]
    public float attackCooldown = 0.7f;
    public float comboWindow = 0.5f; // Time in which a combo can be triggered
    private bool isAttacking = false;
    private bool targetHit = false;
    private bool canCombo = false;
    private bool comboQueued = false;

    [Header("time Settings")]
public float swingDuration = 0.4f;      // 24 frames @ 60 FPS
public float comboSwingDuration = 0.333f; // 20 frames @ 60 FPS
public float comboInputWindow = 0.3f;   // How long you can trigger a combo

    public int damage = 100;
    public int multiplier = 1;

    public Collider hitbox;
    private Coroutine attackCoroutine;

    void Update()
    {
        if (PauseMenu.paused) return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (!isAttacking)
            {
                attackCoroutine = StartCoroutine(PerformAttack(false));
            }
            else if (canCombo)
            {
                comboQueued = true;
            }
        }
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        StopAllCoroutines(); // Ensures all animations, combo logic, etc., are stopped

        isAttacking = false;
        targetHit = false;
        canCombo = false;
        comboQueued = false;

        if (hitbox != null)
            hitbox.enabled = false;

        if (animator != null)
            animator.Play("idle");
    }

    private IEnumerator PerformAttack(bool isCombo)
{
    isAttacking = true;
    targetHit = false;

    if (hitbox != null)
        hitbox.enabled = true;

    // Play animation
    if (animator != null)
        animator.Play(isCombo ? "Swing" : "SwordSwing");

    // Play sound
    if (swingSound != null)
        audioSource.PlayOneShot(swingSound);

    float currentDuration = isCombo ? comboSwingDuration : swingDuration;

    // Wait for most of the animation to pass before enabling combo
    yield return new WaitForSeconds(currentDuration * 0.8f);

    canCombo = true;
    float comboTimer = 0f;

    while (comboTimer < comboInputWindow)
    {
        if (comboQueued)
        {
            comboQueued = false;
            canCombo = false;
            hitbox.enabled = false; // Disable hitbox before next swing
            yield return StartCoroutine(PerformAttack(true));
            yield break;
        }

        comboTimer += Time.deltaTime;
        yield return null;
    }

    canCombo = false;

    if (animator != null)
        animator.Play("idle");

    hitbox.enabled = false;
    yield return new WaitForSeconds(currentDuration * 0.2f); // Wait the rest
    isAttacking = false;
}

    private void OnTriggerEnter(Collider other)
    {
        if (!targetHit && other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            targetHit = true;
            enemy.TakeDamage(damage * multiplier, hitbox.transform.position);
            Debug.Log("hit enemy");
        }
    }
}
