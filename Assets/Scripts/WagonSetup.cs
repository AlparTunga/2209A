using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WagonSetup : MonoBehaviour
{
    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;
    }
}