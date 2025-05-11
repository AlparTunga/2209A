using UnityEngine;

[ExecuteAlways]
public class RopeBetweenPoints : MonoBehaviour
{
    [Tooltip("Ip’in ust noktasi (tavana baglama noktasi)")]
    public Transform topPoint;

    [Tooltip("Ip’in alt noktasi (makara veya agirlik)")]
    public Transform bottomPoint;

    [Tooltip("Ip kalinligi")]
    public float thickness = 0.05f;

    private Vector3 initialScale;

    void Awake()
    {
        // Baslangicta silindirin X,Z kalinligini sakla
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (topPoint == null || bottomPoint == null) return;

        // 1) Iki nokta arasi
        Vector3 dir = bottomPoint.position - topPoint.position;
        float length = dir.magnitude;

        // 2) Pozisyon: orta noktaya tasiyalim
        transform.position = topPoint.position + dir * 0.5f;

        // 3) Rotasyon: Y eksenini dir yonune cevirelim
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);

        // 4) Olcek: Unity Cylinder yuksekligi = 2 birim, bu yuzden scale.y = length/2
        transform.localScale = new Vector3(
            thickness,
            length * 0.5f,
            thickness
        );
    }
}