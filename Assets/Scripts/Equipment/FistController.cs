using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FistController : MonoBehaviour
{
    [Header("Fist")]
    public GameObject fist;
    public int damage = 10;
    public int multiplier = 1;

    [Header("Player")]
    public GameObject player;

    [Header("Hitbox")]
    public GameObject hitbox;

    bool attackedLR = false;

    bool isAttacking = false;

    void Update()
    {
        multiplier = CigController.CurrentDamageMultiplier;
        if (PauseMenu.paused) return;
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // pick the animation based on the last direction
            if (attackedLR)
                StartCoroutine(AttackLeft());
            else
                StartCoroutine(AttackRight());
        }
    }

    IEnumerator AttackLeft()
    {
        hitbox.GetComponent<Collider>().enabled = true;
        isAttacking = true;
        player.GetComponent<Animator>().Play("LeftFist");

        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Animator>().Play("New State");
        attackedLR = false;   // next time we’ll do a right-hand
        isAttacking = false;
        hitbox.GetComponent<Collider>().enabled = false;

    }

    IEnumerator AttackRight()
    {
        hitbox.GetComponent<Collider>().enabled = true;
        isAttacking = true;
        player.GetComponent<Animator>().Play("RightFist");

        yield return new WaitForSeconds(0.5f);
        player.GetComponent<Animator>().Play("New State");
        attackedLR = true;    // next time we’ll do a left-hand
        isAttacking = false;
        hitbox.GetComponent<Collider>().enabled = false;

    }
}
