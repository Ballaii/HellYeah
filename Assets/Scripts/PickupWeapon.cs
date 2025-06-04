using UnityEngine;

public class RocketLauncherPickup : MonoBehaviour
{
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    public int index; // 1 for Rocket Launcher, 2 for SSG
    private bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            EquipmentSwitcher switcher = other.GetComponent<EquipmentSwitcher>();
            if (switcher != null)
            {
                if (index == 1)
                {
                    EquipmentSwitcher.isRocketLauncherUnlocked = true; // Unlock the Rocket Launcher
                    switcher.EquipRocketLauncher(); // Equip the Rocket Launcher immediately
                }
                else if (index == 2)
                {
                    EquipmentSwitcher.isSSGUnlocked = true; // Unlock the SSG
                    switcher.EquipSSG(); // Equip the SSG immediately
                }
            }

            // Optional visual/audio feedback
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            pickedUp = true;
            Destroy(gameObject); // Remove the pickup object
        }
    }
}
