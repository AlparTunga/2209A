using UnityEngine;
using ImGuiNET;

public class EplosionImGuiScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CannonController cannonController;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;

    private bool showIntro = true;

    void OnEnable()  => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        // Icerige gore pencere boyutunu otomatik ayarla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Eginimli Atis Hareketi - Giris", flags);

            ImGui.TextWrapped("Bir cisim egik aciyla atildiginda yatay ve dusey bilesenlerle hareket eder.");
            ImGui.TextWrapped("Maksimum yukseklik, ucus suresi ve menzil hesaplanabilir.");

            ImGui.BulletText("Hmax = (Vy * Vy) / (2 * g)");
            ImGui.BulletText("T    = 2 * Vy / g");
            ImGui.BulletText("R    = V * T * cos(theta)");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        if (cannonController == null || lineRenderer == null)
        {
            ImGui.Begin("Atis Bilgisi", flags);
            ImGui.Text("Referanslar eksik.");
            ImGui.End();
            return;
        }

        ImGui.Begin("Atis Bilgisi", flags);

        // Baslangic hizi
        Vector3 initialVelocity = cannonController.ShotPoint.up * cannonController.BlastPower;
        float totalSpeed        = initialVelocity.magnitude;

        // Acilar ve ucus bilgisi
        Vector3 dir             = cannonController.ShotPoint.up.normalized;
        float verticalAngle     = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        float maxHeight         = (initialVelocity.y * initialVelocity.y) / (2 * gravity);
        float flightTime        = (2 * initialVelocity.y) / gravity;

        // Menzil hesabi
        float range = 0f;
        if (lineRenderer.positionCount >= 2)
        {
            Vector3 start = lineRenderer.GetPosition(0);
            Vector3 end   = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            range = Vector3.Distance(new Vector3(start.x, 0, start.z),
                                     new Vector3(end.x,   0, end.z));
        }

        // Cikis hizi ayari
        ImGui.SliderFloat("Cikis Hizi (BlastPower)", ref cannonController.BlastPower, 0.1f, 100f);

        // Anlik top hizi (varsa)
        Rigidbody cannonballRb = null;
        if (cannonController.LastFiredBall != null)
            cannonballRb = cannonController.LastFiredBall.GetComponent<Rigidbody>();

        // Bilgileri goster
        ImGui.Text($"Y Ekseni Cikis Acisi: {verticalAngle:F2} deg");
        ImGui.Text($"Maks. Yukseklik:      {maxHeight:F2} m");
        ImGui.Text($"Ucus Suresi:          {flightTime:F2} s");
        ImGui.Text($"Menzil:               {range:F2} m");
        ImGui.Text($"Cikis Hizi:           {totalSpeed:F2} m/s");

        if (cannonballRb != null)
            ImGui.Text($"Havadaki Hiz:         {cannonballRb.velocity.magnitude:F2} m/s");
        else
            ImGui.Text("Top henuz firlatilmadi.");

        ImGui.End();
    }
}
