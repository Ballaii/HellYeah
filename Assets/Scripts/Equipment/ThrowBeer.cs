using System.Collections;
using UnityEngine;

public class ThrowBeer : MonoBehaviour
{
    [Header("References")]
    public GameObject item;
    public Transform cam;
    public Transform attackPoint;
    public GameObject projectile;
    public GameObject beer;
    public GameObject leftArm;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse1;
    public float throwForce;
    public float throwUpwardForce;
    public float throwCooldown;

    bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
        attackPoint.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (PauseMenu.paused) return;
        if (Input.GetKeyDown(throwKey) && readyToThrow)
        {
            Throw();
        }
    }

    public IEnumerator Throw()
    {
        item.SetActive(true);
        leftArm.SetActive(false);
        item.GetComponent<Animator>().Play("BeerDrawAnimation");
        yield return new WaitForSeconds(.5f);

        //play anim
        item.GetComponent<Animator>().Play("BeerThrow");
        yield return new WaitForSeconds(.5f);
        //play sound
        //AudioSource audioSource = beer.GetComponent<AudioSource>();

        Animator animator = GetComponent<Animator>();

        // 1) Instantiate your bottle
        GameObject projectileToThrow = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody rb = projectileToThrow.GetComponent<Rigidbody>();

        // 2) Build a ray from the exact screen center
        Camera cameraComp = cam.GetComponent<Camera>();
        Ray ray = cameraComp.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // 3) Aim direction: if we hit something, go toward that point; otherwise just forward
        Vector3 forceDirection = ray.direction;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        // 4) Apply your forces
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
        rb.AddForce(forceToAdd, ForceMode.Impulse);

        // 5) Cooldown & ammo bookkeeping
        readyToThrow = false;
        Invoke(nameof(resetThrow), throwCooldown);
        item.SetActive(false);
        leftArm.SetActive(true);
        animator.Play("New State");
    }



    private void resetThrow()
    {
        readyToThrow = true;
    }
}
