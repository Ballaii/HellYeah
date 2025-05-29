using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles swapping between equipment objects (beer and cigarette) based on key inputs.
/// Attach to the Player or FP_CameraPivot GameObject.
/// </summary>
public class EquipmentSwitcher : MonoBehaviour
{
    [Header("Equipment References")]
    public GameObject beerEquip;
    public GameObject cigEquip;
    public GameObject swordEquip;
    public GameObject shotgunEquip;
    public GameObject grapplingEquip;
    public GameObject barbellEquip;
    public GameObject Fists;

    [Header("Animations")]
    public Animator player;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip beerDrawSound;
    public AudioClip cigDrawSound;
    public AudioClip swordDrawSound;
    public AudioClip shotgunDrawSound;
    public AudioClip grapplingDrawSound;
    public AudioClip barbellDrawSound;

    [Header("Input Keys")]
    //public KeyCode putAwayKey = KeyCode.LeftAlt;
    public KeyCode throwBeerKey = KeyCode.E;
    public KeyCode cigKey = KeyCode.Q;
    public KeyCode drinkBeerKey = KeyCode.F;
    public KeyCode FistsKey = KeyCode.Alpha1;
    public KeyCode swordKey = KeyCode.Alpha2;
    public KeyCode shotgunKey = KeyCode.Alpha3;
    public KeyCode grapplingKey = KeyCode.C;
    public KeyCode barbellKey = KeyCode.Alpha4;

    bool isUsing = false;

    void Start()
    {
        beerEquip.SetActive(false);
        cigEquip.SetActive(false);
        swordEquip.SetActive(false);
        shotgunEquip.SetActive(false);
        grapplingEquip.SetActive(false);
        barbellEquip.SetActive(false);
        Fists.SetActive(true);
}

    void Update()
    {
        if (PauseMenu.paused) return;
        if (Input.GetKeyDown(throwBeerKey))
        {
            
            if (BeerManager.isThrowOnCooldown)
            {
                Debug.Log("Throw is on cooldown.");
                return; // Exit if on cooldown
            }
            if (isUsing)
            {
                Debug.Log("Cannot use");
                return; // Exit if holding other items
            }
            else
            {
                isUsing = true;
                StartCoroutine(beerEquip.GetComponent<ThrowBeer>().Throw());
                BeerManager.Instance.StartCoroutine(BeerManager.Instance.ThrowCooldown(1.4f));
                isUsing = false;
            }
        }
        else if (Input.GetKeyDown(cigKey))
        {
            if (isUsing)
            {
                Debug.Log("Cannot use");
                return; // Exit if holding other items
            }
            else
            {
                isUsing = true;
                StartCoroutine(cigEquip.GetComponent<CigController>().PerformSmoke());
                isUsing = false;
            }
        }
        else if (Input.GetKeyDown(drinkBeerKey))
        {
            if (BeerManager.isDrinkOnCooldown)
            {
                Debug.Log("Drinking is on cooldown.");
                return; // Exit if on cooldown
            }
            if (isUsing)
            {
                Debug.Log("Cannot use");
                return; // Exit if holding other items
            }
            else
            {
                isUsing = true;
                StartCoroutine(beerEquip.GetComponent<BeerDrinker>().PerformTiltDrink());
                BeerManager.Instance.StartCoroutine(BeerManager.Instance.DrinkCooldown(5f));
                isUsing = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (beerEquip != null) beerEquip.SetActive(false);
            if (cigEquip != null) cigEquip.SetActive(false);
            if (swordEquip != null) swordEquip.SetActive(false);
            if (shotgunEquip != null) shotgunEquip.SetActive(false);
            if (grapplingEquip != null) grapplingEquip.SetActive(false);
            if (barbellEquip != null) barbellEquip.SetActive(false);
            if (Fists != null) Fists.SetActive(true);
        }
        else if (Input.GetKeyDown(FistsKey))
        {
            if (Fists.activeSelf) return;
            else StartCoroutine(FistsEquip());
        }
        else if (Input.GetKeyDown(swordKey))
        {
            if (swordEquip.activeSelf) swordEquip.SetActive(false);
            else StartCoroutine(EquipSword());
        }
        else if (Input.GetKeyDown(shotgunKey))
        {
            if (shotgunEquip.activeSelf) shotgunEquip.SetActive(false);
            else StartCoroutine(EquipShotgun());
        }
        else if (Input.GetKeyDown(grapplingKey))
        {
            if (grapplingEquip.activeSelf) grapplingEquip.SetActive(false);
            else StartCoroutine(GrapplingEquip());
        }
        else if (Input.GetKeyDown(barbellKey))
        {
            if (barbellEquip.activeSelf) barbellEquip.SetActive(false);
            else StartCoroutine(BarbellEquip());
        }
    }

    IEnumerator FistsEquip()
    {
        if (beerEquip != null) beerEquip.SetActive(false);
        if (cigEquip != null) cigEquip.SetActive(false);
        if (swordEquip != null) swordEquip.SetActive(false);
        if (shotgunEquip != null) shotgunEquip.SetActive(false);
        if (grapplingEquip != null) grapplingEquip.SetActive(false);
        if (barbellEquip != null) barbellEquip.SetActive(false);
        if (Fists != null) Fists.SetActive(true);
        if (player != null)
            player.GetComponent<Animator>().Play("FistsDraw");

        yield return new WaitForSeconds(0.483f);
        player.GetComponent<Animator>().Play("New State");
    }

    IEnumerator EquipSword()
    {
        if (Fists != null) Fists.SetActive(false);
        if (swordEquip != null) swordEquip.SetActive(true);
        if (shotgunEquip != null) shotgunEquip.SetActive(false);
        if (barbellEquip != null) barbellEquip.SetActive(false);
        // Play swing animation
        if (player != null)
            swordEquip.GetComponent<Animator>().Play("SwordDraw");
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        // Play swing sound
        if (swordDrawSound != null)
        {
            audioSource.PlayOneShot(swordDrawSound);
        }

        yield return new WaitForSeconds(0.667f);
        swordEquip.GetComponent<Animator>().Play("New State");

        
    }

    IEnumerator EquipShotgun()
    {
        if (Fists != null) Fists.SetActive(false);
        if (swordEquip != null) swordEquip.SetActive(false);
        if (shotgunEquip != null) shotgunEquip.SetActive(true);
        if ( barbellEquip != null) barbellEquip.SetActive(false);
        // Play swing animation
        if (player != null)
            shotgunEquip.GetComponent<Animator>().Play("ShotgunDraw");
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        // Play swing sound
        if (shotgunDrawSound != null)
        {
            audioSource.PlayOneShot(shotgunDrawSound);
        }
        yield return new WaitForSeconds(0.267f);
        shotgunEquip.GetComponent<Animator>().Play("New State");

        
    }

    IEnumerator BarbellEquip()
    {
        if (Fists != null) Fists.SetActive(false);
        if(swordEquip != null) swordEquip.SetActive(false);
        if (shotgunEquip != null) shotgunEquip.SetActive(false);
        if (grapplingEquip != null) grapplingEquip.SetActive(false);
        if (barbellEquip != null) barbellEquip.SetActive(true);
        // Play swing animation
        if (player != null)
            player.GetComponent<Animator>().Play("DrawBarbell");
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        // Play swing sound
        if (barbellDrawSound != null)
        {
            audioSource.PlayOneShot(barbellDrawSound);
        }
        yield return new WaitForSeconds(0.4f);
        player.GetComponent<Animator>().Play("New State");
    }

    IEnumerator GrapplingEquip()
    {
        if (Fists != null) Fists.SetActive(false);
        if (grapplingEquip  != null) grapplingEquip.SetActive(true);
        if (beerEquip != null) beerEquip.SetActive(false);
        if (cigEquip != null) cigEquip.SetActive(false);
        if (barbellEquip != null) barbellEquip.SetActive(false);
        if (player != null)
            player.GetComponent<Animator>().Play("DrawGrapple");
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        // Play swing sound
        if (grapplingDrawSound != null)
        {
            audioSource.PlayOneShot(grapplingDrawSound);
        }
        yield return new WaitForSeconds(0.267f);
        player.GetComponent<Animator>().Play("New State");
    }
}