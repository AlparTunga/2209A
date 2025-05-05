using UnityEngine;

public class SeesawSetup : MonoBehaviour
{
    public GameObject seesaw;     // Tahterevalli objesi
    public GameObject pivotPoint; // D�nme noktas� (empty)

    void Start()
    {
        // Rigidbody ekleyip k�tleyi s�f�rla
        Rigidbody rb = seesaw.AddComponent<Rigidbody>();
        rb.mass = 5f;
        rb.angularDrag = 0.05f;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // Hinge Joint olu�tur ve pivot noktas�na ba�la
        HingeJoint hinge = seesaw.AddComponent<HingeJoint>();
        hinge.connectedBody = pivotPoint.AddComponent<Rigidbody>();
        hinge.connectedBody.isKinematic = true;
        hinge.axis = Vector3.forward; // Z ekseninde d�necek (X-Y d�zleminde)

        // D�nme limiti (opsiyonel)
        JointLimits limits = hinge.limits;
        limits.min = -30f;
        limits.max = 30f;
        hinge.limits = limits;
        hinge.useLimits = true;
    }
}
