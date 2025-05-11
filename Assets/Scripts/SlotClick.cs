using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SlotClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        WeightSpawner.PlaceCurrentBlock(transform);
    }
}
