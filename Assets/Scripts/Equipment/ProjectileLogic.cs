using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    public BeerDrinker p;

    private Rigidbody rb;

    private bool targetHit;

    public int damage;
    public int multiplier = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //multiplier = p.isBuffed ? 2 : 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        multiplier = CigController.CurrentDamageMultiplier;
        if (!targetHit)
        {
            targetHit = true;

            if (collision.gameObject.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                enemy.TakeDamage(damage * multiplier, transform.position);

                Destroy(gameObject);
            }

            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        }
    }
}
