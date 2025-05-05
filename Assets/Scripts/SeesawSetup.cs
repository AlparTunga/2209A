using UnityEngine;

public class SeesawSetup : MonoBehaviour
{
    public GameObject seesaw;     // Tahterevalli objesi
    public GameObject pivotPoint; // Dönme noktasý (empty)

    void Start()
    {
        // Rigidbody ekleyip kütleyi sýfýrla
        Rigidbody rb = seesaw.AddComponent<Rigidbody>();
        rb.mass = 5f;
        rb.angularDrag = 0.05f;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // Hinge Joint oluþtur ve pivot noktasýna baðla
        HingeJoint hinge = seesaw.AddComponent<HingeJoint>();
        hinge.connectedBody = pivotPoint.AddComponent<Rigidbody>();
        hinge.connectedBody.isKinematic = true;
        hinge.axis = Vector3.forward; // Z ekseninde dönecek (X-Y düzleminde)

        // Dönme limiti (opsiyonel)
        JointLimits limits = hinge.limits;
        limits.min = -30f;
        limits.max = 30f;
        hinge.limits = limits;
        hinge.useLimits = true;
    }
}
