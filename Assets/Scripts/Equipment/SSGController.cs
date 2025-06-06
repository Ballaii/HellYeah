using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Improved SSGController: uses VelocityChange for pellets, spherical spread, and reliable collisions.
/// </summary>
public class SSGController : MonoBehaviour
{
    [Header("Ammo & Firing")]
    [Tooltip("Number of pellets per shot.")]
    public int pelletCount = 16;
    [Tooltip("Spread radius in degrees.")]
    public float spreadAngle = 35f;
    [Tooltip("Initial speed of each pellet.")]
    public float pelletSpeed = 2500f;
    [Tooltip("Time before each pellet is destroyed.")]
    public float pelletLifetime = 1.5f;
    [Tooltip("Prefab with Rigidbody and collision logic.")]
    public GameObject pelletPrefab;
    [Tooltip("Muzzle transform for pellet spawn.")]
    public Transform muzzlePoint1;
    public Transform muzzlePoint2;

    
    [Tooltip("Shots per second.")]
    public float fireRate = 1f;
    public GameObject muzzleFlash;
    public Light shotlight;

    List<Quaternion> pellets;

    [Header("Recoil Settings")]
    public Transform shotgunTransform;
    public Vector3 recoilOffset = new Vector3(0f, 0f, -0.1f);
    public float recoilDuration = 0.3f;

    [Header("Pump Settings")]
    public float rotation = 0.4f;
    public float reloadDuration = 1.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public AudioClip emptyClickClip;

    [Header("Ammunition")]
    [Tooltip("Maximum shells in the shotgun.")]
    public int maxAmmo = 5;
    [Tooltip("Current loaded shells.")]
    public int currentAmmo = 5;
    [Tooltip("Time it takes to reload one shell.")]
    public float reloadTimePerShell = 1.2f;
    private bool isReloading = false;

    private Vector3 shotgunOriginalRotation;
    private Camera playerCamera;

    private bool onCooldown = false;

    void Awake()
    {
        muzzlePoint1.rotation = Quaternion.Euler(0f, 235f, 0f);
        muzzlePoint2.rotation = Quaternion.Euler(0f, 235f, 0f);
        pellets = new List<Quaternion>(new Quaternion[pelletCount]);

        playerCamera = Camera.main;
        shotgunOriginalRotation = shotgunTransform.localRotation.eulerAngles;
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (PauseMenu.paused) return;
        if (Input.GetButtonDown("Fire1") && !isReloading && !onCooldown)
        {
            if (currentAmmo > 0)
                StartCoroutine(FireShotgun());
            else
                PlayEmptyClick();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
            StartCoroutine(Reload());

    }

    private void PlayEmptyClick()
    {
        if (emptyClickClip != null)
            audioSource.PlayOneShot(emptyClickClip);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        while (currentAmmo < maxAmmo && isReloading)
        {
            if (reloadClip != null)
                audioSource.PlayOneShot(reloadClip);

            currentAmmo++;
        }
        yield return new WaitForSeconds(reloadTimePerShell);

        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }

    public void CancelReload()
    {
        isReloading = false;
    }

    private IEnumerator FireShotgun()
    {
        if (currentAmmo <= 0) yield break;
        onCooldown = true;
        currentAmmo--;
        audioSource.PlayOneShot(shotClip);


        shotlight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        shotlight.enabled = false;
        GameObject muzzleF = (GameObject)Instantiate(muzzleFlash, muzzlePoint1.position, muzzlePoint1.rotation);
        GameObject muzzleF2 = (GameObject)Instantiate(muzzleFlash, muzzlePoint2.position, muzzlePoint2.rotation);
        Destroy(muzzleF, 0.1f);
        Destroy(muzzleF2, 0.1f);

        GameObject pel = new GameObject();
        

        for (int i = 0; i < pelletCount; i++)
        {
            pellets[i] = Random.rotation;
            pel = (GameObject)Instantiate(pelletPrefab, muzzlePoint1.position, muzzlePoint1.rotation) as GameObject;
            Destroy(pel, pelletLifetime);
            pel.transform.rotation = Quaternion.RotateTowards(pel.transform.rotation, pellets[i], spreadAngle);
            pel.GetComponent<Rigidbody>().AddForce(pel.transform.forward * pelletSpeed);
        }
        for (int i = 0; i < pelletCount; i++)
        {
            pellets[i] = Random.rotation;
            pel = (GameObject)Instantiate(pelletPrefab, muzzlePoint2.position, muzzlePoint2.rotation) as GameObject;
            Destroy(pel, pelletLifetime);
            pel.transform.rotation = Quaternion.RotateTowards(pel.transform.rotation, pellets[i], spreadAngle);
            pel.GetComponent<Rigidbody>().AddForce(pel.transform.forward * pelletSpeed);
        }

        // Recoil & pump
        //yield return DoRecoil();
        yield return DoPump();

        yield return new WaitForSeconds(fireRate);
        onCooldown = false;
    }

    private void PlayReload()
    {
        audioSource.PlayOneShot(reloadClip);

        StartCoroutine(Reload());
    }

    private IEnumerator DoRecoil()
    {
        Vector3 start = shotgunOriginalRotation;
        Vector3 target = start + recoilOffset;
        yield return LerpLocalPos(shotgunTransform, start, target, recoilDuration);
        yield return LerpLocalPos(shotgunTransform, target, start, recoilDuration);
    }

    private IEnumerator DoPump()
    {
        PlayReload();
     
        GetComponent<Animator>().Play("Showcase");
        yield return new WaitForSeconds(reloadDuration);
        GetComponent<Animator>().Play("Idle");
    }

    private IEnumerator LerpLocalPos(Transform t, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            t.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        t.localPosition = to;
    }
}
