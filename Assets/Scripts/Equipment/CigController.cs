using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to your Cigarette prefab (child of the camera).
/// Right‐click to “smoke”: consumes one cig, applies a global damage multiplier,
/// flashes screen tint, then reverts after duration.
/// Other scripts should multiply their damage by CigController.CurrentDamageMultiplier.
/// </summary>
public class CigController : MonoBehaviour
{
    public GameObject item;
    [Header("Smoke Effects")]
    [Tooltip("Damage multiplier applied when smoking.")]
    public int damageMultiplier = 2;
    [Tooltip("Duration of the damage boost, in seconds.")]
    public float boostDuration = 10f;

    public AudioClip cigSound;
    public AudioSource audioSource;

    // Static so all scripts can see current multiplier
    public static int CurrentDamageMultiplier { get; private set; } = 1;

    private bool isSmoking = false;

    void Update()
    {
        if (PauseMenu.paused) return;

        // Right mouse button
        if (Input.GetMouseButtonDown(1) && !isSmoking)
        {
            StartCoroutine(PerformSmoke());
        }
    }

    public static void SetDamageMultiplier(int newMultiplier)
    {
        CurrentDamageMultiplier = newMultiplier;
    }

    public IEnumerator PerformSmoke()
    {
        item.SetActive(true);
        GetComponent<Animator>().Play("CigDrawAnimation");
        yield return new WaitForSeconds(.5f);
        isSmoking = true;

        // aanim
        GetComponent<Animator>().Play("CigSmoke");
        audioSource.PlayOneShot(cigSound);

        // Apply global damage multiplier
        // Tell the manager to handle the boost timing:
        CigManager.Instance.StartDamageBoost(damageMultiplier, boostDuration);

        // wait
        yield return new WaitForSeconds(.9f);

        GetComponent<Animator>().Play("New State");

        isSmoking = false;
        item.SetActive(false);
        
    }

    private IEnumerator EndDamageBoost()
    {
        yield return new WaitForSeconds(boostDuration);
        CurrentDamageMultiplier = 1;
    }
}
