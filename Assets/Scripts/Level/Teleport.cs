using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform tp;
    [SerializeField] private GameObject player;

    private bool isPlayerInside = false;
    private Coroutine tpCoroutine = null;

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the designated player
        if (other.gameObject == player)
        {
            isPlayerInside = true;
            // Start the teleport countdown
            tpCoroutine = StartCoroutine(TeleportAfterDelay(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInside = false;
            // Cancel the teleport if they leave too early
            if (tpCoroutine != null)
            {
                StopCoroutine(tpCoroutine);
                tpCoroutine = null;
            }
        }
    }

    private IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Only teleport if player is still inside
        if (isPlayerInside)
        {
            player.transform.position = tp.position;
        }

        tpCoroutine = null;
    }
}
