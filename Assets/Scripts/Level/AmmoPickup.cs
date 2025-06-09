using System.Collections;
using UnityEngine;

/// <summary>
/// Attach this script to an ammo pickup prefab with a trigger collider and kinematic Rigidbody.
/// When the player enters the trigger, it adds ammo to their ThrowBeer component and destroys itself.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AmmoPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of ammo to add when picked up.")]
    public int ammoAmount;
    [Tooltip("Tag of the player to check for pickup.")]
    public string playerTag = "Player";
    [Tooltip("Audio clip to play on pickup.")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    public enum AmmoType
    {
        RocketLauncher,
        SuperShotgun
    }

    public AmmoType ammoType;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            //check for ammo type
            switch (ammoType)
            {
                case AmmoType.RocketLauncher:
                    RocketLauncher rocketLauncher = other.GetComponentInChildren<RocketLauncher>();

                    if (rocketLauncher != null)
                    {
                        rocketLauncher.AddAmmo(ammoAmount);

                    }
                    else
                    {
                        Debug.LogWarning("RocketLauncher component not found on player.");
                    }
                    break;
                case AmmoType.SuperShotgun:
                    SSGController superShotgun = other.GetComponent<SSGController>();
                    if (superShotgun != null)
                    {
                        superShotgun.AddAmmo(ammoAmount);
                    }
                    else
                    {
                        Debug.LogWarning("SuperShotgun component not found on player.");
                    }
                    break;
            }

            StartCoroutine(PickupAndDestroy());
        }
    }

    IEnumerator PickupAndDestroy()
{
    if (pickupSound != null)
    {
        audioSource.PlayOneShot(pickupSound);
        yield return new WaitForSeconds(pickupSound.length);
    }
    Destroy(gameObject);
}


}