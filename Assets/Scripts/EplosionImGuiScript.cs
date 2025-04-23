using UnityEngine;
using ImGuiNET;

public class EplosionImGuiScript : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private CannonController cannonController;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Fizik")]
    [SerializeField] private float gravity = 9.81f;

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
        if (cannonController == null || lineRenderer == null)
        {
            ImGui.Begin("Atış Bilgisi");
            ImGui.Text("Referanslar eksik.");
            ImGui.End();
            return;
        }

        ImGui.Begin("Atış Bilgisi");

        // Başlangıç hızı
        Vector3 initialVelocity = cannonController.ShotPoint.up * cannonController.BlastPower;
        float totalSpeed = initialVelocity.magnitude;

        // Açılar ve uçuş bilgisi
        Vector3 dir = cannonController.ShotPoint.up.normalized;
        float verticalAngle = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        float maxHeight = (initialVelocity.y * initialVelocity.y) / (2 * gravity);
        float flightTime = (2 * initialVelocity.y) / gravity;

        // Menzil
        float range = 0f;
        if (lineRenderer.positionCount >= 2)
        {
            Vector3 start = lineRenderer.GetPosition(0);
            Vector3 end = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            range = Vector3.Distance(new Vector3(start.x, 0, start.z), new Vector3(end.x, 0, end.z));
        }

        // Çıkış hızı ayarı
        ImGui.SliderFloat("Çıkış Hızı (BlastPower)", ref cannonController.BlastPower, 0.1f, 100f);

        // Anlık hız (sahnede varsa)
        Rigidbody cannonballRigidbody = null;
        if (cannonController.LastFiredBall != null)
        {
            cannonballRigidbody = cannonController.LastFiredBall.GetComponent<Rigidbody>();
        }

        // Bilgiler
        ImGui.Text($"Y Ekseninde Çıkış Açısı: {verticalAngle:F2}°");
        ImGui.Text($"Maks. Yükseklik: {maxHeight:F2} m");
        ImGui.Text($"Uçuş Süresi: {flightTime:F2} s");
        ImGui.Text($"Menzil: {range:F2} m");
        ImGui.Text($"Çıkış Hızı: {totalSpeed:F2} m/s");

        if (cannonballRigidbody != null)
        {
            float currentSpeed = cannonballRigidbody.velocity.magnitude;
            ImGui.Text($"Havadaki Hız: {currentSpeed:F2} m/s");
        }
        else
        {
            ImGui.Text("Top henüz fırlatılmadı.");
        }

        ImGui.End();
    }
}
