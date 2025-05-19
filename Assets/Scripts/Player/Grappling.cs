using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private CPMPlayer pm;  // Changed from PlayerMovementGrappling to CPMPlayer
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public GameObject grapplingEquip;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;
    public AudioClip grappleClip;
    public AudioSource grappleSource;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        // 1) CPMPlayer
        pm = GetComponent<CPMPlayer>();
        if (pm == null)
            Debug.LogError("Grappling: no CPMPlayer component found on this GameObject!");

        // 2) Camera
        if (cam == null && Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else if (cam == null)
        {
            Debug.LogError("Grappling: Camera Transform not assigned and no Camera.main found.");
        }

        // 3) Gun Tip
        if (gunTip == null)
            Debug.LogError("Grappling: Gun Tip Transform not assigned.");

        // 4) Line Renderer
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
            if (lr == null)
                Debug.LogWarning("Grappling: No LineRenderer found or assigned — rope won't draw.");
        }

    }


    private void Update()
    {
        //Check if grappling gun is active

        if (!grapplingEquip.activeSelf) return;
        if (Input.GetKeyDown(grappleKey)) StartGrapple();
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling && lr != null)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;
        //pm.freeze = true;

        if (grappleClip != null)
        {
            grappleSource.PlayOneShot(grappleClip);
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        // Enable line renderer if available
        if (lr != null)
        {
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }
    }

    private void ExecuteGrapple()
    {
        //pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
            highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        //pm.freeze = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;

        // Disable line renderer if available
        if (lr != null)
            lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}