using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Manages the player's health, applies fall damage, and triggers screen bob on damage. Attach to the player GameObject.
/// Requires a CharacterController component on the same GameObject to detect grounding.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    int maxHealth = 100;
    public Slider HPSlider;
    public int currentHealth;
    public AudioClip hurt;
    public AudioClip death;
    public AudioSource audioSource;
    public GameOver gameOver;
    public Transform playerCam;

    void Start()
    {
        currentHealth = maxHealth;
        HPSlider.maxValue = maxHealth;
        HPSlider.value = currentHealth;
    }

    private void Update()
    {
        HPSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Die());
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        audioSource.PlayOneShot(hurt);
    }

    IEnumerator Die()
    {
        audioSource.PlayOneShot(death);

        // capture the start and end rotations
        Quaternion startRot = playerCam.rotation;
        Quaternion endRot = Quaternion.Euler(0f, 0f, 90f);

        float duration = 0.5f;     // how long the tilt should take
        float elapsed = 0f;

        // over duration seconds, interpolate from start to end
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // you can use Slerp or Lerp; Slerp gives a more “constant-speed” feel
            playerCam.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ensure final rotation
        playerCam.rotation = endRot;

        // give them a moment to see the tilt
        yield return new WaitForSeconds(1f);

        // Death anim / game over
        gameOver.GameOverScreen();

        // …and respawn (if you have that)
    }
}