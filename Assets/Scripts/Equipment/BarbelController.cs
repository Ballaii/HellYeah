using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BarbelController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator; // Animator controlling the sword swing animation
    public AudioSource audioSource; // AudioSource for playing sounds
    public AudioClip swingSound;
    public AudioClip curlSound;
    public AudioClip error;

    [Header("Attack Settings")]
    public float attackCooldown = 0.7f; // Time between attacks
    private bool isAttacking = false;
    bool targetHit = false;
    public Collider hitbox;

    [Header("Projectile Settings")]
    public Camera cam;
    private Vector3 destination;
    public Transform atkPoint;
    public GameObject projectile;

    public int damage = 100;
    public int multiplier = 1;

    void Start()
    {
        hitbox.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        multiplier = CigController.CurrentDamageMultiplier;
        if (PauseMenu.paused) return;
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            StartCoroutine(Melee());
        }

        if (Input.GetButtonDown("Fire2") && !isAttacking)
        {
            StartCoroutine(Ranged());
        }
    }

    private System.Collections.IEnumerator Ranged()
    {
        isAttacking = true;


        // Play swing animation
        if (animator != null)
            animator.GetComponent<Animator>().Play("CurlBB");
        else
        {
            Debug.LogError("Animator is not assigned.");
            audioSource.PlayOneShot(error);
        }
        
        StartCoroutine(ShootProjectile());

        // Play swing sound
        if (curlSound != null)
        {
            audioSource.PlayOneShot(curlSound);
        }

        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(attackCooldown);

        animator.GetComponent<Animator>().Play("New State");

        isAttacking = false;
    }

    private System.Collections.IEnumerator Melee()
    {
        isAttacking = true;

        hitbox.GetComponent<Collider>().enabled = true;

        // Play swing animation
        if (animator != null)
            animator.GetComponent<Animator>().Play("SwingBB");
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

        animator.GetComponent<Animator>().Play("New State");
        hitbox.GetComponent<Collider>().enabled = false;
        isAttacking = false;
    }

    IEnumerator ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else destination = ray.GetPoint(500);

        yield return new WaitForSeconds(0.5f);

        InstantiateProjectile();
    }
    void InstantiateProjectile()
    {
        var projectileToThrow = Instantiate(projectile, atkPoint.position, Quaternion.identity) as GameObject;
        projectileToThrow.GetComponent<Rigidbody>().linearVelocity = (destination - atkPoint.position).normalized * 20f;

        Destroy(projectileToThrow, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // bail if we already hit
        if (targetHit) return;

        // see if *any* parent has an Enemy script
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            targetHit = true;
            Debug.Log("hit enemy");
            enemy.TakeDamage(damage * multiplier, hitbox.transform.position);
            StartCoroutine(ResetHitFlag());
        }
    }

    private IEnumerator ResetHitFlag()
    {
        // wait one FixedUpdate so you don’t immediately re-hit the same collider
        yield return new WaitForFixedUpdate();
        targetHit = false;
    }

}
