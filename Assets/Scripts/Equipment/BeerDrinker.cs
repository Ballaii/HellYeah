using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to your Beer prefab (child of the camera).
/// Middle click tilts the beer toward the center and back, consumes one beer, applies speed boost and screen tint.
/// </summary>
public class BeerDrinker : MonoBehaviour
{
    [Header("Drink Effects")]
    [Tooltip("Movement speed multiplier when drinking.")]
    int HealthBack;
    int maxHealth = 100;

    [Header("Reference")]
    public GameObject Item;
    public ThrowBeer beer;
    public PlayerHealth playerHealth;
    public AudioClip drinkSound;
    public AudioSource source;
    public GameObject leftArm;

    private bool isDrinking = false;
    private int health;

    void Update()
    {
        health = playerHealth.currentHealth;
        if (PauseMenu.paused) return;
        if (Input.GetMouseButtonDown(2) && !isDrinking)
            StartCoroutine(PerformTiltDrink());
    }

    public IEnumerator PerformTiltDrink()
    {
        // hp
        if (beer == null || playerHealth.currentHealth < 0)
        {
            Debug.Log("No beers left to drink!");
            yield break;
        }
        leftArm.SetActive(false);
        beer.item.SetActive(true);
        beer.item.GetComponent<Animator>().Play("BeerDrawAnimation");
        yield return new WaitForSeconds(.5f);

        isDrinking = true;

        // anim
        Animator animator = GetComponent<Animator>();
        beer.item.GetComponent<Animator>().Play("BeerDrink");
        source.PlayOneShot(drinkSound);

        if (health > 0 && health <= 100)
        {
            HealthBack = maxHealth - health;
            health += HealthBack;
            playerHealth.currentHealth = health;
        }

        yield return new WaitForSeconds(.9f);
        animator.Play("New State");

        isDrinking = false;
        leftArm.SetActive(true);
        beer.item.SetActive(false);
    }
}