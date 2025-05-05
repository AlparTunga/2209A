using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MassBlock : MonoBehaviour
{
    public float blockMass = 5f;

    private Rigidbody rb;
    private bool hasBeenPlaced = false;

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = blockMass;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void OnMouseDown()
    {
        if (!hasBeenPlaced)
        {
            PlacementManager.Instance.SetSelectedBlock(this);
        }
    }

    public void PlaceAt(Vector3 point)
    {
        transform.position = new Vector3(point.x, fixedY, fixedZ);

        rb.isKinematic = false;
        rb.useGravity = true;

        hasBeenPlaced = true;
    }
}
