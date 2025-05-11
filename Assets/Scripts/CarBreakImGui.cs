using UnityEngine;
using ImGuiNET;

public class CarPhysicsImGui : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private PhysicMaterial trackMaterial;
    [SerializeField] private CarBreak carBreakScript;
    [Header("Baslangic Transform")]
    [SerializeField] private Vector3 startPosition        = new Vector3(-19.47f, 11.74f, 0f);
    [SerializeField] private Vector3 startRotationEuler   = new Vector3(-60f, 90f, -90f);

    private Quaternion startRotationQuat;
    private bool showIntro = true;

    private void Awake()
    {
        startRotationQuat = Quaternion.Euler(startRotationEuler);
    }

    private void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
    }

    private void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

    private void OnLayout()
    {
        // Pencereyi icerige gore otomatik boyutla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Arac Dinamigi ve Frenleme - Giris", flags);

            ImGui.TextWrapped("Egitimli bir yuzeyde arac, egim kuvveti ve fren kuvveti arasinda dengede durmaya calisir.");
            ImGui.TextWrapped("Fren kuvvetine bagli olarak aracin durma suresi ve mesafesi hesaplanabilir.");

            ImGui.BulletText("F_egim    = m * g * sin(theta)");
            ImGui.BulletText("F_fren    = m * a                  (Fren kuvveti)");
            ImGui.BulletText("t_durus   = v / a                  (Durma suresi)");
            ImGui.BulletText("d_durus   = v * v / (2 * a)        (Durma mesafesi)");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        ImGui.Begin("Araba Fizigi Paneli", flags);

        // Araba kutlesi
        float mass = carRigidbody.mass;
        if (ImGui.SliderFloat("Araba Kutlesi", ref mass, 0.1f, 2000f))
            carRigidbody.mass = mass;

        // Fren kuvveti
        float breakForce = carBreakScript._breakForceMagnitude;
        if (ImGui.SliderFloat("Fren Kuvveti", ref breakForce, 0f, 10000f))
            carBreakScript._breakForceMagnitude = breakForce;

        // Denge kuvveti secenegi
        bool useEquilibrium = carBreakScript._isUsingEquilibriumForceWhenBreaking;
        if (ImGui.Checkbox("Denge Kuvveti Kullan", ref useEquilibrium))
            carBreakScript._isUsingEquilibriumForceWhenBreaking = useEquilibrium;

        // Eğim kuvveti hesapla
        float slopeAngleDeg = carRigidbody.transform.rotation.eulerAngles.x;
        if (slopeAngleDeg > 180f) slopeAngleDeg -= 360f;
        float slopeAngleRad = slopeAngleDeg * Mathf.Deg2Rad;
        float slopeForce    = carRigidbody.mass * -Physics.gravity.y * Mathf.Sin(-slopeAngleRad);
        Vector3 slopeVector = carRigidbody.transform.right * slopeForce;

        // Fren kuvveti vektoru
        Vector3 breakVector = Input.GetMouseButton(0)
            ? -carBreakScript._breakForceMagnitude * carRigidbody.transform.right
            : Vector3.zero;

        // Net kuvvet
        Vector3 netForce = slopeVector + breakVector;

        ImGui.Text($"Egim Acisi (deg):      {slopeAngleDeg:F2}");
        ImGui.Text($"Egim Kuvveti (X):      {slopeVector.x:F2} N");
        ImGui.Text($"Fren Kuvveti (X):      {breakVector.x:F2} N");
        ImGui.Separator();
        ImGui.Text($"Net Kuvvet (X):        {netForce.x:F2} N");

        // Arac hizi
        float velocity = carRigidbody.velocity.magnitude;
        ImGui.Text($"Anlik Hiz:             {velocity:F2} m/s");

        // Durma suresi ve mesafesi
        if (!useEquilibrium && breakForce > 0f)
        {
            float acceleration = breakForce / carRigidbody.mass;
            float stopTime     = velocity / acceleration;
            float stopDistance = (velocity * velocity) / (2f * acceleration);
            ImGui.Text($"Tahmini Durus Suresi:  {stopTime:F2} s");
            ImGui.Text($"Tahmini Durus Mesafesi:{stopDistance:F2} m");
        }

        // Reset butonu
        if (ImGui.Button("Reset Araba"))
        {
            carRigidbody.velocity        = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            carRigidbody.transform.position = startPosition;
            carRigidbody.transform.rotation = startRotationQuat;
            carRigidbody.useGravity         = true;
        }

        ImGui.End();
    }
}
