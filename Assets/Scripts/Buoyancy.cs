using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoyancy : MonoBehaviour
{
    [Header("Fluid Properties")]
    [Tooltip("Sıvının yoğunluğu (kg/m³)")]
    public float fluidDensity = 1000f;

    [Header("References")]
    [Tooltip("Sahnedeki su hacmi objesinin Transform'u")]
    public Transform waterVolume;

    [Header("Sampling Points")]
    public Transform[] samplePoints;
    [Tooltip("Bir nokta tamamen batmışsa maxDepth kadar su altındadır")]
    public float maxDepth = 1f;

    [Header("Damping")]
    public float linearDrag = 0.5f;
    public float angularDrag = 0.5f;

    // Küp hacmi ve kütlesi
    float objectVolume;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Eğer su Volume referansı atanmamışsa otomatik bul
        if (waterVolume == null)
        {
            var go = GameObject.Find("WaterVolume");
            if (go) waterVolume = go.transform;
        }
    }

    void Start()
    {
        // Küp ölçeğinden hacmi al (1×1×1 için 1)
        Vector3 s = transform.localScale;
        objectVolume = s.x * s.y * s.z;

        // Rigidbody sürtünmeleri
        rb.drag        = linearDrag;
        rb.angularDrag = angularDrag;
    }

    void FixedUpdate()
    {
        ApplyBuoyancyForces();
    }

    void LateUpdate()
    {
        // Her kare sonunda doğru yüksekliğe çek
        FloatAtSurface();
    }

    void ApplyBuoyancyForces()
    {
        if (waterVolume == null) return;

        float surfaceY = waterVolume.position.y + waterVolume.localScale.y * 0.5f;
        float g = Physics.gravity.magnitude;

        foreach (var pt in samplePoints)
        {
            // Noktanın ne kadar su altında kaldığını hesapla
            float depth = surfaceY - pt.position.y;
            if (depth <= 0f) continue;
            float immersion = Mathf.Clamp01(depth / maxDepth);

            // Kaldırma
            Vector3 F = fluidDensity * objectVolume * immersion * -Physics.gravity;
            rb.AddForceAtPosition(F, pt.position);
        }
    }

    void FloatAtSurface()
    {
        if (waterVolume == null) return;

        float sY    = waterVolume.position.y + waterVolume.localScale.y * 0.5f;
        float bY    = waterVolume.position.y - waterVolume.localScale.y * 0.5f;
        float h     = transform.localScale.y;
        float mass  = rb.mass;
        float g     = Physics.gravity.magnitude;

        // Blok yoğunluğu ve sıvı yoğunluğu
        float blockDensity = mass / objectVolume;
        float ratio        = blockDensity / fluidDensity;

        float newY;
        if (ratio < 1f)
        {
            // kısmen yüzer: immersion = ratio
            float submerged = ratio * h;
            float bottomPos = sY - submerged;
            newY = bottomPos + h * 0.5f;
        }
        else
        {
            // tamamen batar
            newY = bY + h * 0.5f;
        }

        // Taşıyıcıyı sabitler
        var p = transform.position;
        rb.transform.position = new Vector3(p.x, newY, p.z);
        rb.velocity            = Vector3.zero;
        rb.angularVelocity     = Vector3.zero;
    }
}
