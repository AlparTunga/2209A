using UnityEngine;

[ExecuteAlways]
public class SlotAligner : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Dönen seesaw objesinin Transform'u (pivot noktası)")]
    public Transform seesawTransform;
    [Tooltip("Seesaw'ın MeshFilter bileşeni (spacing için)")]
    public MeshFilter seesawMeshFilter;

    [Header("Slot Ayarları")]
    [Tooltip("Toplam slot sayısı (örn. 7)")]
    public int slotCount = 7;
    [Range(0f, 0.49f), Tooltip("Mesh genişliğinin uçlarından kesme oranı")]
    public float sideMarginFraction = 0f;
    [Tooltip("Seesaw yüzeyinin üzerinde ne kadar yüksekte dursun")]
    public float yOffset = 0.01f;
    [Tooltip("Seesaw pivot Z düzlemine göre ofset")]
    public float zOffset = 0f;

    void Update()
    {
        if (seesawTransform == null || seesawMeshFilter == null) return;
        if (slotCount < 2 || transform.childCount < slotCount) return;

        // 1) Mesh'in dünya-ölçeğindeki genişliği
        float meshWidth = seesawMeshFilter.sharedMesh.bounds.size.x
                        * seesawTransform.lossyScale.x;
        // 2) Uçlardan kırpma bölgesi
        float margin = meshWidth * sideMarginFraction;
        float effectiveWidth = Mathf.Max(0f, meshWidth - 2f * margin);
        // 3) Slot spacing
        float spacing = effectiveWidth / (slotCount - 1);
        int half = slotCount / 2;

        // 4) Orta nokta seesaw pivot X,Y,Z
        Vector3 pivotPos = seesawTransform.position;

        // 5) Her slot'u konumlandır
        for (int i = 0; i < slotCount; i++)
        {
            int idx = i - half;              // -half ... +half
            float localX = idx * spacing;    // seesaw Local X içinde
            Vector3 localPos = new Vector3(localX, yOffset, zOffset);
            // seesawTransform.TransformPoint ile rotasyonu ve pivot'un pozunu alıyoruz
            Vector3 worldPos = seesawTransform.TransformPoint(localPos);
            transform.GetChild(i).position = worldPos;
        }
    }
}