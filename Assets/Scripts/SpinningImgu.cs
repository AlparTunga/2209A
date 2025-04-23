using UnityEngine;
using ImGuiNET;

public class SpinningImgu : MonoBehaviour
{
    [SerializeField] private Rigidbody[] objects; // Cisimlerin Rigidbody bileşenleri
    [SerializeField] private SpinObjects spinObjects; // Torku yöneten nesne
    private float globalTorque = 10f; // Tüm kutular için ortak tork
    private float changeStep = 1f; // Değişim miktarı seçimi

    void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
    }

    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

    void OnLayout()
    {
        ImGui.Begin("Dönme Hareketi Simülatörü");

        // Değişim miktarını seçme butonları
        ImGui.Text("Degisim Miktari:");
        if (ImGui.Button("0.1")) changeStep = 0.1f;
        ImGui.SameLine();
        if (ImGui.Button("1")) changeStep = 1f;
        ImGui.SameLine();
        if (ImGui.Button("10")) changeStep = 10f;
        ImGui.SameLine();
        if (ImGui.Button("100")) changeStep = 100f;

        // Global tork değeri için + ve - butonları
        ImGui.Text("Genel Tork: " + globalTorque);
        if (ImGui.Button("-##tork")) globalTorque -= changeStep;
        ImGui.SameLine();
        if (ImGui.Button("+##tork")) globalTorque += changeStep;
        globalTorque = Mathf.Clamp(globalTorque, 0f, 100f);
        spinObjects.SetTorque(globalTorque);

        foreach (var obj in objects)
        {
            Vector3 scale = obj.transform.localScale;
            float mass = obj.mass;
            float inertia = (1f / 12f) * mass * (scale.x * scale.x + scale.y * scale.y); // Dikdörtgen/kare için I
            float angularVelocity = obj.angularVelocity.magnitude;

            ImGui.Text($"Nesne: {obj.gameObject.name}");
            
            // Kütle değiştirici (+/- butonları)
            ImGui.Text($"Kütle: {mass:F1} kg");
            if (ImGui.Button($"-##kütle{obj.gameObject.name}")) mass -= changeStep;
            ImGui.SameLine();
            if (ImGui.Button($"+##kütle{obj.gameObject.name}")) mass += changeStep;
            mass = Mathf.Clamp(mass, 0.1f, 500f);
            obj.mass = mass;

            // Boyut değiştirici (+/- butonları, max limit kaldırıldı)
            ImGui.Text($"Genislik (X): {scale.x:F2}");
            if (ImGui.Button($"-##genislik{obj.gameObject.name}")) scale.x -= changeStep;
            ImGui.SameLine();
            if (ImGui.Button($"+##genislik{obj.gameObject.name}")) scale.x += changeStep;
            scale.x = Mathf.Max(0.1f, scale.x);

            ImGui.Text($"Yükseklik (Y): {scale.y:F2}");
            if (ImGui.Button($"-##yukseklik{obj.gameObject.name}")) scale.y -= changeStep;
            ImGui.SameLine();
            if (ImGui.Button($"+##yukseklik{obj.gameObject.name}")) scale.y += changeStep;
            scale.y = Mathf.Max(0.1f, scale.y);

            obj.transform.localScale = scale;
            
            ImGui.Text($"Eylemsizlik Momenti (I): {inertia:F2}");
            ImGui.Text($"Acisal Hiz : {angularVelocity:F2}");
            ImGui.Separator();
        }

        // Tüm kutuları durdurma butonu
        if (ImGui.Button("Tüm Kutulari Durdur"))
        {
            foreach (var obj in objects)
            {
                obj.velocity = Vector3.zero;
                obj.angularVelocity = Vector3.zero;
            }
        }

        ImGui.End();
    }
}
