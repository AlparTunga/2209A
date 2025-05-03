// Assets/Scripts/PendulumImGui.cs
using UnityEngine;
using ImGuiNET;
using System.Reflection;

public class PendulumImGui : MonoBehaviour
{
    [Tooltip("Sahnede var olan PendulumController'ı buraya sürükleyin.")]
    public PendulumController pendulum;

    void OnEnable()  => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        ImGui.Begin("Pendulum Controls");

        if (pendulum == null)
        {
            ImGui.TextColored(new Vector4(1f, 0f, 0f, 1f),
                              "PendulumController atanmamış!");
            ImGui.End();
            return;
        }

        // — Parametre Slider’ları —
        float v;
        v = pendulum.length;
        if (ImGui.SliderFloat("Length", ref v, 0.1f, 10f)) pendulum.length = v;
        v = pendulum.mass;
        if (ImGui.SliderFloat("Mass", ref v, 0.1f, 50f)) pendulum.mass = v;
        v = pendulum.gravity;
        if (ImGui.SliderFloat("Gravity", ref v, 0f, 20f)) pendulum.gravity = v;
        v = pendulum.friction;
        if (ImGui.SliderFloat("Friction", ref v, 0f, 5f)) pendulum.friction = v;
        v = pendulum.initialAngle;
        if (ImGui.SliderFloat("Initial Angle", ref v, 0f, 90f)) pendulum.initialAngle = v;
        v = pendulum.initialForce;
        if (ImGui.SliderFloat("Initial Force", ref v, 0f, 20f)) pendulum.initialForce = v;

        // — Reset Butonu —
        if (ImGui.Button("Reset"))
        {
            var type = pendulum.GetType();
            var angleF = type.GetField("angle", BindingFlags.NonPublic|BindingFlags.Instance);
            var velF   = type.GetField("angularVelocity", BindingFlags.NonPublic|BindingFlags.Instance);
            if (angleF != null) angleF.SetValue(pendulum, pendulum.initialAngle * Mathf.Deg2Rad);
            if (velF   != null) velF.SetValue(pendulum, pendulum.initialForce / (pendulum.mass * pendulum.length));
            var upd = type.GetMethod("UpdatePosition", BindingFlags.NonPublic|BindingFlags.Instance);
            upd?.Invoke(pendulum, null);
        }

        // — Energy Graph Bölümü —
        if (ImGui.CollapsingHeader("Energy Graph"))
        {
            var type  = pendulum.GetType();
            float angle = (float)(type.GetField("angle", BindingFlags.NonPublic|BindingFlags.Instance)
                                   ?.GetValue(pendulum) ?? 0f);
            float w     = (float)(type.GetField("angularVelocity", BindingFlags.NonPublic|BindingFlags.Instance)
                                   ?.GetValue(pendulum) ?? 0f);
            float m = pendulum.mass, L = pendulum.length, g = pendulum.gravity;
            float KE = 0.5f * m * (w * L) * (w * L);
            float PE = m * g * L * (1f - Mathf.Cos(angle));
            float total = KE + PE;

            ImGui.Text($"Kinetic Energy: {KE:F2} J");
            ImGui.ProgressBar(total > 0 ? KE/total : 0, new Vector2(200,20), "");
            ImGui.Text($"Potential Energy: {PE:F2} J");
            ImGui.ProgressBar(total > 0 ? PE/total : 0, new Vector2(200,20), "");
        }

        ImGui.End();
    }
}
