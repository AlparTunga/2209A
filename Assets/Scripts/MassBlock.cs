using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class MassBlock : MonoBehaviour
{
    public float blockMass = 5f;

    private Rigidbody rb;
    private Collider col;
    private bool hasBeenPlaced = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        rb.mass = blockMass;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        // Başlangıçta tam kilitliyoruz; PlaceOnSlot açacak X-Z’yi
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    /// <summary>
    /// SlotClick tarafından çağrılır; bloğu slot pozuna taşır,
    /// X-Z eksenini kilitleyip Y serbest bırakır, collider’ı açık tutar.
    /// </summary>
    public void PlaceOnSlot(Transform slot)
    {
        if (hasBeenPlaced || slot == null) return;

        // 1) Pozisyonu slot’a eşitle
        transform.position = slot.position;

        // 2) Physics’i aç: Y serbest, XZ sabit, rotasyon sabit
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        // 3) Collider açık kalsın, böylece ağırlık seesaw’a aktarılır
        col.enabled = true;

        hasBeenPlaced = true;
    }

    public bool HasBeenPlaced() => hasBeenPlaced;
}