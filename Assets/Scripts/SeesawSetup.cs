using UnityEngine;

[DisallowMultipleComponent]
public class SeesawSetup : MonoBehaviour
{
    [Header("Referanslar")]
    public GameObject seesaw;       // Tahta objesi
    public Transform pivotPoint;    // Dönme noktası

    [Header("Rigidbody Ayarları")]
    [Tooltip("Daha düşük kütle → daha hızlı tepki")]
    public float seesawMass = 5f;       // Önceden 10f

    [Tooltip("Daha düşük drag → daha serbest salınım, ama çok düşük yapmayın")]
    public float angularDrag = 0.5f;     // Önceden 1.5f

    [Header("Hinge Limit & Spring")]
    public float minAngle = -20f;
    public float maxAngle = 20f;

    [Tooltip("Yüksek spring → daha sert ve hızlı toparlama")]
    public float springForce = 100f;     // Önceden 50f

    [Tooltip("Düşük damper → daha çabuk salınım, ama çok azaltmayın yoksa zıplar")]
    public float springDamper = 5f;       // Önceden 20f

    void Start()
    {
        // 1) Tahtanın Rigidbody'si
        var rb = seesaw.GetComponent<Rigidbody>()
                 ?? seesaw.AddComponent<Rigidbody>();
        rb.mass = seesawMass;
        rb.useGravity = false;
        rb.angularDrag = angularDrag;
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationY;

        // 2) HingeJoint
        var hinge = seesaw.GetComponent<HingeJoint>()
                    ?? seesaw.AddComponent<HingeJoint>();
        hinge.useLimits = true;
        hinge.autoConfigureConnectedAnchor = false;
        hinge.connectedBody = null;
        hinge.anchor = seesaw.transform
                                          .InverseTransformPoint(pivotPoint.position);
        hinge.connectedAnchor = pivotPoint.position;
        hinge.axis = Vector3.forward;

        var limits = hinge.limits;
        limits.min = minAngle;
        limits.max = maxAngle;
        hinge.limits = limits;

        // 3) Spring ayarları
        hinge.useSpring = true;
        var spring = hinge.spring;
        spring.spring = springForce;
        spring.damper = springDamper;
        spring.targetPosition = 0f;  // Hedef 0° (düz)
        hinge.spring = spring;

        hinge.useMotor = false;
    }
}
