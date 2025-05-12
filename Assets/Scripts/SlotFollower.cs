using UnityEngine;

public class SlotFollower : MonoBehaviour
{
    [Header("Seesaw Referansý")]
    [Tooltip("Tahtanýn pivot merkezli Transform'u")]
    public Transform seesaw;

    [Header("Ofset Ayarlarý")]
    [Tooltip("Pivot noktasýndan local X ekseninde uzaklýk")]
    public float xOffsetFromPivot;
    [Tooltip("Pivot noktasýndan local Z ekseninde uzaklýk")]
    public float zOffset = 0f;
    [Tooltip("Seesaw yüzeyinin üzerinde ne kadar yukarýda dursun")]
    public float verticalOffset = 0.01f;

    void LateUpdate()
    {
        if (seesaw == null) return;

        // 1) Seesaw'ýn local eksenine göre ofsetli nokta
        Vector3 localPoint = new Vector3(xOffsetFromPivot, 0f, zOffset);

        // 2) Dünyaya çevir
        Vector3 worldPoint = seesaw.TransformPoint(localPoint);

        // 3) Yukarýya bindir ve ata
        transform.position = worldPoint + seesaw.up * verticalOffset;
    }
}
