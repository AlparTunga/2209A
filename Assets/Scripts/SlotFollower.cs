using UnityEngine;

public class SlotFollower : MonoBehaviour
{
    [Header("Seesaw Referans�")]
    [Tooltip("Tahtan�n pivot merkezli Transform'u")]
    public Transform seesaw;

    [Header("Ofset Ayarlar�")]
    [Tooltip("Pivot noktas�ndan local X ekseninde uzakl�k")]
    public float xOffsetFromPivot;
    [Tooltip("Pivot noktas�ndan local Z ekseninde uzakl�k")]
    public float zOffset = 0f;
    [Tooltip("Seesaw y�zeyinin �zerinde ne kadar yukar�da dursun")]
    public float verticalOffset = 0.01f;

    void LateUpdate()
    {
        if (seesaw == null) return;

        // 1) Seesaw'�n local eksenine g�re ofsetli nokta
        Vector3 localPoint = new Vector3(xOffsetFromPivot, 0f, zOffset);

        // 2) D�nyaya �evir
        Vector3 worldPoint = seesaw.TransformPoint(localPoint);

        // 3) Yukar�ya bindir ve ata
        transform.position = worldPoint + seesaw.up * verticalOffset;
    }
}
