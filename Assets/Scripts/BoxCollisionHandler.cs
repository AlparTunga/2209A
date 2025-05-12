using UnityEngine;

public class BoxCollisionHandler : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollisionImguı manager;

    public void Init(BoxCollisionImguı mgr)
    {
        manager = mgr;
        rb      = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        var otherRb = collision.rigidbody;
        if (otherRb == null || !manager.IsManagedBox(otherRb.gameObject)) return;

        float m1 = rb.mass, m2 = otherRb.mass;
        Vector3 v1 = rb.velocity, v2 = otherRb.velocity;
        Vector3 normal = (otherRb.position - rb.position).normalized;
        float vr = Vector3.Dot(v1 - v2, normal);

        // her kutudaki material.bounciness kullanılıyor
        float e = rb.GetComponent<Collider>().material.bounciness;
        float j = -(1 + e) * vr / (1/m1 + 1/m2);

        Vector3 v1A = v1 + (j/m1) * normal;
        Vector3 v2A = v2 - (j/m2) * normal;

        manager.LogCollision(j, v1A, v2A);
    }
}
