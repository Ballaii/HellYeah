using UnityEngine;

/// <summary>
/// Attach this script to an ammo pickup prefab with a trigger collider and kinematic Rigidbody.
/// When the player enters the trigger, it adds ammo to their ThrowBeer component and destroys itself.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AmmoPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [Tooltip("Amount of ammo to add when picked up.")]
    public int ammoAmount = 5;

    private void Reset()
    {
        // Ensure this collider is a trigger by default
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    
}