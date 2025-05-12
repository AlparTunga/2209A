using UnityEngine;

public class FloorFrictionManager : MonoBehaviour
{
    public PhysicMaterial iceMaterial;
    public PhysicMaterial normalMaterial;
    public PhysicMaterial roughMaterial;

    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
        SetIceFloor(); // Başlangıç zemini
    }

    public void SetIceFloor() => col.material = iceMaterial;
    public void SetNormalFloor() => col.material = normalMaterial;
    public void SetRoughFloor() => col.material = roughMaterial;
}