using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Improved ShotgunController: uses VelocityChange for pellets, spherical spread, and reliable collisions.
/// </summary>
public class ShotgunController : MonoBehaviour
{
    [Header("Ammo & Firing")]
    [Tooltip("Number of pellets per shot.")]
    public int pelletCount = 8;
    [Tooltip("Spread radius in degrees.")]
    public float spreadAngle = 45f;
    [Tooltip("Initial speed of each pellet.")]
    public float pelletSpeed = 50f;
    [Tooltip("Time before each pellet is destroyed.")]
    public float pelletLifetime = 1.5f;
    [Tooltip("Prefab with Rigidbody and collision logic.")]
    public GameObject pelletPrefab;
    [Tooltip("Muzzle transform for pellet spawn.")]
    public Transform muzzlePoint;
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
    public Transform pumpTransform;
    public float pumpForwardZ = 0.002f;
    public float pumpBackwardZ = 0.003f;
    public float pumpDuration = 1.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public AudioClip emptyClickClip;

    [Header("Ammunition")]
    [Tooltip("Maximum shells in the shotgun.")]
    public int maxAmmo = 8;
    [Tooltip("Current loaded shells.")]
    public int currentAmmo = 8;
    [Tooltip("Time it takes to reload one shell.")]
    public float reloadTimePerShell = 1.2f;
    private bool isReloading = false;

    //private bool readyToFire = true;
    private Vector3 shotgunOriginalPos;
    private Vector3 pumpOriginalLocalPos;
    private Camera playerCamera;

    void Awake()
    {
        pellets = new List<Quaternion>(new Quaternion[pelletCount]);

        playerCamera = Camera.main;
        shotgunOriginalPos = shotgunTransform.localPosition;
        pumpOriginalLocalPos = pumpTransform.localPosition;
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (PauseMenu.paused) return;
        if (Input.GetButtonDown("Fire1") && !isReloading)
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
            yield return DoPump();
        }
        yield return new WaitForSeconds(reloadTimePerShell);

        isReloading = false;
    }

    public void CancelReload()
    {
        isReloading = false;
    }

    private IEnumerator FireShotgun()
    {
        //readyToFire = false;
        currentAmmo--;
        audioSource.PlayOneShot(shotClip);


        shotlight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        shotlight.enabled = false;
        GameObject muzzleF = (GameObject)Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(muzzleF, 0.1f);

        GameObject pel = new GameObject();
        for (int i = 0; i < pelletCount; i++)
        {
            pellets[i] = Random.rotation;
            pel = (GameObject)Instantiate(pelletPrefab, muzzlePoint.position, muzzlePoint.rotation) as GameObject;
            Destroy(pel, pelletLifetime);
            pel.transform.rotation = Quaternion.RotateTowards(pel.transform.rotation, pellets[i], spreadAngle);
            pel.GetComponent<Rigidbody>().AddForce(pel.transform.forward * pelletSpeed);
        }

        // Recoil & pump
        yield return DoRecoil();
        yield return DoPump();

        yield return new WaitForSeconds(1f/fireRate);
        //readyToFire = true;
    }

    private void PlayReload()
    {
        audioSource.PlayOneShot(reloadClip);
    }

    private IEnumerator DoRecoil()
    {
        Vector3 start = shotgunOriginalPos;
        Vector3 target = start + recoilOffset;
        yield return LerpLocalPos(shotgunTransform, start, target, recoilDuration);
        yield return LerpLocalPos(shotgunTransform, target, start, recoilDuration);
    }

    private IEnumerator DoPump()
    {
        audioSource.PlayOneShot(reloadClip);

        Vector3 start = pumpOriginalLocalPos;
        Vector3 forward = new Vector3(start.x, start.y, pumpForwardZ);
        Vector3 back = new Vector3(start.x, start.y, pumpBackwardZ);
        yield return LerpLocalPos(pumpTransform, forward, back, pumpDuration);
        yield return LerpLocalPos(pumpTransform, back, forward, pumpDuration);
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
