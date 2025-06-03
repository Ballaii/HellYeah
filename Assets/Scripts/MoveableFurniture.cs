using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveableFurniture : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        rb.AddTorque(Random.insideUnitSphere * explosionForce * 0.1f, ForceMode.Impulse);
    }
}
