using UnityEngine;
using ImGuiNET;

public class CarPhysicsImGui : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private PhysicMaterial trackMaterial; // sadece UI kontrolü için
    [SerializeField] private CarBreak carBreakScript;
    [SerializeField] private Vector3 startPosition = new Vector3(-19.47f, 11.74f, 0f);

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
        ImGui.Begin("Araba Fizigi Paneli");

        // Kutle
        float mass = carRigidbody.mass;
        if (ImGui.SliderFloat("Araba Kutlesi", ref mass, 0.1f, 2000f))
            carRigidbody.mass = mass;

        // Fren gucu
        float breakForce = carBreakScript._breakForceMagnitude;
        if (ImGui.SliderFloat("Fren Kuvveti", ref breakForce, 0f, 10000f))
            carBreakScript._breakForceMagnitude = breakForce;

        // Denge kuvveti kullanimi
        bool useEquilibrium = carBreakScript._isUsingEquilibriumForceWhenBreaking;
        if (ImGui.Checkbox("Denge Kuvveti Kullan", ref useEquilibrium))
            carBreakScript._isUsingEquilibriumForceWhenBreaking = useEquilibrium;

        // Egim
        float slopeAngleDeg = carRigidbody.transform.rotation.eulerAngles.x;
        if (slopeAngleDeg > 180f) slopeAngleDeg -= 360f;
        float slopeAngleRad = slopeAngleDeg * Mathf.Deg2Rad;

        float slopeForce = mass * -Physics.gravity.y * Mathf.Sin(-slopeAngleRad);
        Vector3 slopeVector = carRigidbody.transform.right * slopeForce;

        Vector3 breakVector = Input.GetMouseButton(0)
            ? -carBreakScript._breakForceMagnitude * carRigidbody.transform.right
            : Vector3.zero;

        Vector3 netForce = slopeVector + breakVector;

        // Gosterimler
        ImGui.Text($"Egim Acisi (deg): {slopeAngleDeg:F2}");
        ImGui.Text($"Egim Kuvveti (X): {slopeVector.x:F2} N");
        ImGui.Text($"Fren Kuvveti (X): {breakVector.x:F2} N");
        ImGui.Separator();
        ImGui.Text($"Net Kuvvet (X): {netForce.x:F2} N");

        // Anlik hiz ve tahmini durma
        float velocity = carRigidbody.velocity.magnitude;
        ImGui.Text($"Anlik Hiz: {velocity:F2} m/s");

        if (!useEquilibrium && breakForce > 0f)
        {
            float acceleration = breakForce / mass;
            float stopTime = velocity / acceleration;
            float stopDistance = (velocity * velocity) / (2f * acceleration);
            ImGui.Text($"Tahmini Durus Suresi: {stopTime:F2} s");
            ImGui.Text($"Tahmini Durus Mesafesi: {stopDistance:F2} m");
        }

        // Reset
        if (ImGui.Button("Reset Araba"))
        {
            carRigidbody.velocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            carRigidbody.position = startPosition;
            carRigidbody.rotation = Quaternion.identity;
            carRigidbody.useGravity = true;
        }

        ImGui.End();
    }
}
