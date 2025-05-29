using System.Collections;
using UnityEngine;

public class CigManager : MonoBehaviour
{
    public static CigManager Instance { get; private set; }
     public Transparency transparencyScript;

     public static bool isOnCooldown = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Kicks off a one‐off damage boost, then resets it automatically.
    /// </summary>
    public void StartDamageBoost(int multiplier, float duration)
    {
        // Immediately apply
        CigController.SetDamageMultiplier(2);

        // Begin countdown
        StartCoroutine(EndBoostAfter(duration));
        // Start the transparency fade
        StartCoroutine(Cooldown(duration));
    }

    private IEnumerator EndBoostAfter(float secs)
    {
        yield return new WaitForSeconds(secs);
        CigController.SetDamageMultiplier(1);

    }

    public IEnumerator Cooldown(float duration)
    {
        isOnCooldown = true;
        int steps = 10;
        float stepTime = duration / steps;
        float alphaStart = 25;
        float alphaEnd = 255;
        float alphaStep = (alphaEnd - alphaStart) / steps;

        if (transparencyScript != null)
        {
            transparencyScript.alpha = alphaStart;

            for (int i = 1; i <= steps; i++)
            {
                yield return new WaitForSeconds(stepTime);
                transparencyScript.alpha = alphaStart + alphaStep * i;
            }
        }
        isOnCooldown = false;

    }
}
