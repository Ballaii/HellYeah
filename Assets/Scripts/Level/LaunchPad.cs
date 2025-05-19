using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class LaunchPad : MonoBehaviour
{
    [Tooltip("How many times higher the player can jump while on this pad.")]
    public float jumpBoostMultiplier = 2.0f;

    // Map each boosted player to their original jumpSpeed
    private Dictionary<CPMPlayer, float> _originalJumpSpeeds = new Dictionary<CPMPlayer, float>();

    private void Reset()
    {
        // Ensure your table’s collider is a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var cpm = other.GetComponent<CPMPlayer>();
        if (cpm != null && !_originalJumpSpeeds.ContainsKey(cpm))
        {
            // store the original, then boost
            _originalJumpSpeeds[cpm] = cpm.jumpSpeed;
            cpm.jumpSpeed *= jumpBoostMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var cpm = other.GetComponent<CPMPlayer>();
        float original;
        if (cpm != null && _originalJumpSpeeds.TryGetValue(cpm, out original))
        {
            // restore exactly what we saved
            cpm.jumpSpeed = original;
            _originalJumpSpeeds.Remove(cpm);
        }
    }
}
