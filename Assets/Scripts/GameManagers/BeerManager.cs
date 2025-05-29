using System.Collections;
using UnityEngine;

public class BeerManager : MonoBehaviour
{
    public static BeerManager Instance { get; private set; }
    public Transparency transparencyScriptDrink;
    public Transparency transparencyScriptThrow;

    public static bool isThrowOnCooldown = false;
    public static bool isDrinkOnCooldown = false;
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

    public IEnumerator ThrowCooldown(float duration)
    {
        isThrowOnCooldown = true;
        int steps = 10;
        float stepTime = duration / steps;
        float alphaStart = 25;
        float alphaEnd = 255;
        float alphaStep = (alphaEnd - alphaStart) / steps;

        if (transparencyScriptThrow != null)
        {
            transparencyScriptThrow.alpha = alphaStart;

            for (int i = 1; i <= steps; i++)
            {
                yield return new WaitForSeconds(stepTime);
                transparencyScriptThrow.alpha = alphaStart + alphaStep * i;
            }
        }
        isThrowOnCooldown = false;

    }
    
     public IEnumerator DrinkCooldown(float duration)
    {
        isDrinkOnCooldown = true;
        int steps = 10;
        float stepTime = duration / steps;
        float alphaStart = 25;
        float alphaEnd = 255;
        float alphaStep = (alphaEnd - alphaStart) / steps;

        if (transparencyScriptDrink != null)
        {
            transparencyScriptDrink.alpha = alphaStart;

            for (int i = 1; i <= steps; i++)
            {
                yield return new WaitForSeconds(stepTime);
                transparencyScriptDrink.alpha = alphaStart + alphaStep * i;
            }
        }
        isDrinkOnCooldown = false;

    }
}
