using UnityEngine;

public class ShockwaveProjectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject impactVFX;

    public int damage = 100;
    private bool targetHit;
    public int multiplier = 1;

    private void OnCollisionEnter(Collision collision)
    {
        multiplier = CigController.CurrentDamageMultiplier;
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Projectile")
        {

            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity) as GameObject;
            if (!targetHit)
            {
                targetHit = true;

                Enemy enemy = collision.collider.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage * multiplier, impactVFX.transform.position);

                    Destroy(gameObject);
                }
            }

            Destroy(impact,2f);

            Destroy(gameObject);


        }
    }
}
