using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Reads player stats (speed, health, ammo) and updates UI elements.
/// Attach this to a UI Canvas GameObject.
/// </summary>
public class PlayerUIManager : MonoBehaviour
{
    [Header("Player Reference")]
    public CharacterController playerController;  // for speed
    public PlayerHealth playerHealth;             // custom health script
    public ThrowBeer throwBeer;                   // script holding ammo info
    public ShotgunController shotgun;              // script holding ammo info

    [Header("UI Elements")]
    public Text speedText;   // e.g. "Speed: 0.0"
    public Text healthText;  // e.g. "HP: 100"
    public Text ammoText;    // e.g. "Ammo: 5"
    public Text shotgunAmmoText;    // e.g. "Ammo: 6"

    void Update()
    {
        UpdateSpeed();
        UpdateShotgunAmmo();
    }

    private void UpdateSpeed()
    {
        if (playerController != null && speedText != null)
        {
            // horizontal speed
            Vector3 horizontalVel = playerController.velocity;
            horizontalVel.y = 0;
            float speed = horizontalVel.magnitude;
            speedText.text = $"Speed: {speed:F1}";
        }
    }

  

   

    private void UpdateShotgunAmmo()
    {
        if (shotgun != null && shotgunAmmoText != null)
        {
            shotgunAmmoText.text = $"Ammo: {shotgun.currentAmmo}/{shotgun.maxAmmo}";
        }
    }
}
