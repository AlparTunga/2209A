using UnityEngine;
using ImGuiNET;
using System.Reflection;

public class PendulumImGui : MonoBehaviour
{
    [Tooltip("Sahnede var olan PendulumController'i buraya surukleyin.")]
    public PendulumController pendulum;

    private bool showIntro = true;

    void OnEnable()  => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        // Pencereyi icerige gore otomatik boyutla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Basit Sarkac - Giris", flags);

            ImGui.TextWrapped("Sarkac, yercekimi etkisiyle periyodik salini yapan bir sistemdir.");
            ImGui.TextWrapped("Enerji kinetik ve potansiyel enerji arasinda donusur.");

            ImGui.BulletText("T   = 2*pi * sqrt(L / g)           (Periyot)");
            ImGui.BulletText("KE  = 0.5 * m * (w * L) * (w * L) (Kinetik Enerji)");
            ImGui.BulletText("PE  = m * g * L * (1 - cos(a))     (Potansiyel Enerji)");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        if (pendulum == null)
        {
            ImGui.Begin("Sarkac Kontrolleri", flags);
            ImGui.Text("PendulumController atanmamis!");
            ImGui.End();
            return;
        }

        ImGui.Begin("Sarkac Kontrolleri", flags);

        float v;
        v = pendulum.length;
        if (ImGui.SliderFloat("Uzunluk", ref v, 0.1f, 10f)) pendulum.length = v;
        v = pendulum.mass;
        if (ImGui.SliderFloat("Kutle", ref v, 0.1f, 50f)) pendulum.mass = v;
        v = pendulum.gravity;
        if (ImGui.SliderFloat("Yercekimi", ref v, 0f, 20f)) pendulum.gravity = v;
        v = pendulum.friction;
        if (ImGui.SliderFloat("Surutme", ref v, 0f, 5f)) pendulum.friction = v;
        v = pendulum.initialAngle;
        if (ImGui.SliderFloat("Baslangic Aci", ref v, 0f, 90f)) pendulum.initialAngle = v;
        v = pendulum.initialForce;
        if (ImGui.SliderFloat("Baslangic Kuvvet", ref v, 0f, 20f)) pendulum.initialForce = v;

        if (ImGui.Button("Reset"))
        {
            var type       = pendulum.GetType();
            var angleField = type.GetField("angle",           BindingFlags.NonPublic | BindingFlags.Instance);
            var velField   = type.GetField("angularVelocity", BindingFlags.NonPublic | BindingFlags.Instance);
            if (angleField != null) angleField.SetValue(pendulum, pendulum.initialAngle * Mathf.Deg2Rad);
            if (velField   != null) velField.SetValue(pendulum, pendulum.initialForce   / (pendulum.mass * pendulum.length));
            var upd = type.GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
            upd?.Invoke(pendulum, null);
        }

        if (ImGui.CollapsingHeader("Enerji Grafigi"))
        {
            var type  = pendulum.GetType();
            float a   = (float)(type.GetField("angle",           BindingFlags.NonPublic | BindingFlags.Instance)
                                  ?.GetValue(pendulum) ?? 0f);
            float w   = (float)(type.GetField("angularVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
                                  ?.GetValue(pendulum) ?? 0f);
            float m   = pendulum.mass;
            float L   = pendulum.length;
            float g   = pendulum.gravity;
            float KE  = 0.5f * m * (w * L) * (w * L);
            float PE  = m * g * L * (1f - Mathf.Cos(a));
            float tot = KE + PE;

            ImGui.Text($"Kinetik Enerji:    {KE:F2} J");
            ImGui.ProgressBar(tot > 0 ? KE / tot : 0, new Vector2(200,20), "");
            ImGui.Text($"Potansiyel Enerji: {PE:F2} J");
            ImGui.ProgressBar(tot > 0 ? PE / tot : 0, new Vector2(200,20), "");
        }

        ImGui.End();
    }
}
