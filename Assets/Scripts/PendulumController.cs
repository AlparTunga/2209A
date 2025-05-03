using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(Rigidbody))]
public class PendulumController : MonoBehaviour
{
    [Header("Referanslar")]
    [Tooltip("Sarkaç bağlantı noktası (bağlanacak Transform)")]
    public Transform pivot;

    [Header("Simülasyon Parametreleri")]
    [Tooltip("İp uzunluğu")]
    public float length       = 2f;
    [Tooltip("Kütle")]
    public float mass         = 1f;
    [Tooltip("Yerçekimi")]
    public float gravity      = 9.81f;
    [Tooltip("Sönüm katsayısı")]
    public float friction     = 0.1f;
    [Tooltip("Başlangıç açısı (°)")]
    public float initialAngle = 30f;
    [Tooltip("Başlangıç yatay kuvveti (N)")]
    public float initialForce = 5f;

    private LineRenderer rope;
    private float angle;           // açısal konum (radyan)
    private float angularVelocity; // açısal hız

    void Awake()
    {
        // Rigidbody'yi kinematik yap, ama kütleyi ata
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.mass        = mass;
        }

        // LineRenderer'ı al veya ekle
        rope = GetComponent<LineRenderer>();
        if (rope == null)
            rope = gameObject.AddComponent<LineRenderer>();

        rope.positionCount = 2;
        rope.startWidth    = 0.05f;
        rope.endWidth      = 0.05f;
        rope.useWorldSpace = true;
        rope.material      = new Material(Shader.Find("Sprites/Default"));
    }

    void Start()
    {
        // başlangıç açısı ve hız
        angle           = initialAngle * Mathf.Deg2Rad;
        angularVelocity = initialForce / (mass * length);
        UpdatePosition();
    }

    void Update()
    {
        // pivot veya rope eksikse hiç hesaplama yapma
        if (pivot == null || rope == null)
            return;

        // sarkaç denklemi: θ'' = -(g/L)*sinθ - (c/m)*θ'
        float alpha = -(gravity / length) * Mathf.Sin(angle)
                      - (friction  / mass)  * angularVelocity;

        angularVelocity += alpha * Time.deltaTime;
        angle          += angularVelocity * Time.deltaTime;

        UpdatePosition();
    }

    void UpdatePosition()
    {
        // pivot veya rope eksikse çık
        if (pivot == null || rope == null)
            return;

        // offset ve anchor
        Vector3 offset    = new Vector3(
            Mathf.Sin(angle),
            -Mathf.Cos(angle),
            0f
        ) * length;
        Vector3 anchorPos = pivot.position + offset;

        // rotasyonu ayarla (küpün up ekseni ipin yönünde)
        transform.up = offset.normalized;

        // küpün tepe noktası hesapla
        float halfH = transform.localScale.y * 0.5f;
        transform.position = anchorPos - transform.up * halfH;

        // ipi çiz
        rope.SetPosition(0, pivot.position);
        rope.SetPosition(1, anchorPos);
    }

    void OnValidate()
    {
        // Editör modunda pivot veya rope yoksa çık
        if (pivot == null)
            return;

        // play modda çalışırken hemen pozisyon güncelle
        if (!Application.isPlaying)
            return;

        // rope yeniden ata (örneğin Inspector’dan silinmişse)
        if (rope == null)
            rope = GetComponent<LineRenderer>();

        UpdatePosition();
    }
}
