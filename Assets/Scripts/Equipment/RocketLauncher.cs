using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Ammo & Firing")]
    public int maxAmmo = 10;
    public int currentAmmo = 10;
    public int rocketCount = 1;
    public float rocketSpeed = 100f;
    public float rocketLifetime = 5f;
    public GameObject rocketPrefab;
    public Transform muzzlePoint;
    public float fireRate = 1f;
    public GameObject muzzleFlash;
    public Light shotlight;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shotClip;

    private bool onCooldown = false;

    void Update()
    {
        
        if (Input.GetButtonDown("Fire1") && !onCooldown && currentAmmo > 0)
        {
            FireRocket();
        }
        
    }

    void FireRocket()
    {
        if (currentAmmo <= 0) return;
        if (rocketPrefab == null || muzzlePoint == null)
        {
            Debug.LogError("Rocket prefab or muzzle point is missing.");
            return;
        }

        for (int i = 0; i < rocketCount; i++)
        {
            GameObject rocket = Instantiate(rocketPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzlePoint.forward * rocketSpeed;
            }

            // Rocket script already handles its own destruction
        }

        currentAmmo--;
        if (currentAmmo < 0)
        {
            currentAmmo = 0;
        }
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation);
        }

        if (shotlight != null)
        {
            shotlight.enabled = true;
            Invoke(nameof(DisableShotLight), 0.1f);
        }

        if (audioSource != null && shotClip != null)
        {
            audioSource.PlayOneShot(shotClip);
        }

        onCooldown = true;
        Invoke(nameof(ResetCooldown), 1f / fireRate);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;

        }
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }

    void DisableShotLight()
    {
        if (shotlight != null)
        {
            shotlight.enabled = false;
        }
    }
}
