using System.Collections;
using UnityEngine;

public class CigManager : MonoBehaviour
{
    public static CigManager Instance { get; private set; }

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
    }

    private IEnumerator EndBoostAfter(float secs)
    {
        yield return new WaitForSeconds(secs);
        CigController.SetDamageMultiplier(1);

    }
}
