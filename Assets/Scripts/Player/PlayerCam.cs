using System.Collections;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // Camera settings
    private Camera cam;
    private float defaultFov;
    public float fovChangeDuration = 0.25f; // Time to transition between FOV values

    private Coroutine fovCoroutine;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("PlayerCam script needs to be attached to a GameObject with a Camera component!");
            return;
        }

        defaultFov = cam.fieldOfView;
    }

    // Change camera FOV with smooth transition
    public void DoFov(float targetFov)
    {
        // Stop any running FOV changes
        if (fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }

        fovCoroutine = StartCoroutine(ChangeFov(targetFov));
    }

    // Coroutine to smoothly transition FOV
    private IEnumerator ChangeFov(float targetFov)
    {
        float startFov = cam.fieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < fovChangeDuration)
        {
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, elapsedTime / fovChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.fieldOfView = targetFov;
        fovCoroutine = null;
    }

    // Reset FOV to default
    public void ResetFov()
    {
        DoFov(defaultFov);
    }
}