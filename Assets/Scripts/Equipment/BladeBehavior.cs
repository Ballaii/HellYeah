using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BladeBehavior : MonoBehaviour
{
    public int damage = 90;
    private bool targetHit;
    public int multiplier = 1;
    public AudioSource audioSource;
    public AudioClip hitSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        multiplier = CigController.CurrentDamageMultiplier;
        // bail if we already hit
        if (targetHit) return;

        // see if *any* parent has an Enemy script
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            targetHit = true;
            Debug.Log("hit enemy");
            enemy.TakeDamage(damage * multiplier, transform.position);

            audioSource.PlayOneShot(hitSound);

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