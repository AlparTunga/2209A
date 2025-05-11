using UnityEngine;
using ImGuiNET;

public class SpinningImgu : MonoBehaviour
{
    [SerializeField] private Rigidbody[] objects;      // Cisimlerin Rigidbody bileşenleri
    [SerializeField] private SpinObjects spinObjects;  // Torku yoneten nesne

    private float globalTorque = 10f;  // Tum cisimler icin ortak tork
    private float changeStep    = 1f;  // Degisim miktari secimi

    private bool showIntro = true;

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
        // Icerige gore pencere boyutunu otomatik ayarla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Donme Hareketi ve Eylemsizlik - Giris", flags);

            ImGui.TextWrapped("Bir cisme tork uygulandiginda donme hareketi baslar.");
            ImGui.TextWrapped("Eylemsizlik momenti cismin donmeye karsi gosterdigi direncin olcusudur.");

            ImGui.BulletText("tau     = I * alpha               (Tork = Eylemsizlik * Acisal ivme)");
            ImGui.BulletText("I       = (1/12) * m * (w^2 + h^2) (Dikdortgen icin eylemsizlik momenti)");
            ImGui.BulletText("KE_rot  = 0.5 * I * omega^2       (Donme kinetik enerjisi)");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        ImGui.Begin("Donme Hareketi Simulatoru", flags);

        // Degisim miktarini sec
        ImGui.Text("Degisim Miktari:");
        if (ImGui.Button("0.1")) changeStep = 0.1f;
        ImGui.SameLine();
        if (ImGui.Button("1"))   changeStep = 1f;
        ImGui.SameLine();
        if (ImGui.Button("10"))  changeStep = 10f;
        ImGui.SameLine();
        if (ImGui.Button("100")) changeStep = 100f;

        // Global tork ayari
        ImGui.Text($"Genel Tork: {globalTorque:F1}");
        if (ImGui.Button("-##tork")) globalTorque = Mathf.Max(0f, globalTorque - changeStep);
        ImGui.SameLine();
        if (ImGui.Button("+##tork")) globalTorque = Mathf.Min(100f, globalTorque + changeStep);
        spinObjects.SetTorque(globalTorque);

        // Her cisim icin ozellikler
        foreach (var obj in objects)
        {
            Vector3 scale       = obj.transform.localScale;
            float   mass        = obj.mass;
            float   inertia     = (1f / 12f) * mass * (scale.x * scale.x + scale.y * scale.y);
            float   angularVel  = obj.angularVelocity.magnitude;

            ImGui.Separator();
            ImGui.Text($"Nesne: {obj.gameObject.name}");

            // Kutle
            ImGui.Text($"Kutle: {mass:F1} kg");
            if (ImGui.Button($"-##kutle{obj.name}")) mass = Mathf.Max(0.1f, mass - changeStep);
            ImGui.SameLine();
            if (ImGui.Button($"+##kutle{obj.name}")) mass += changeStep;
            obj.mass = mass;

            // X ekseni olcek
            ImGui.Text($"Genislik (X): {scale.x:F2}");
            if (ImGui.Button($"-##genislik{obj.name}")) scale.x = Mathf.Max(0.1f, scale.x - changeStep);
            ImGui.SameLine();
            if (ImGui.Button($"+##genislik{obj.name}")) scale.x += changeStep;

            // Y ekseni olcek
            ImGui.Text($"Yukseklik (Y): {scale.y:F2}");
            if (ImGui.Button($"-##yukseklik{obj.name}")) scale.y = Mathf.Max(0.1f, scale.y - changeStep);
            ImGui.SameLine();
            if (ImGui.Button($"+##yukseklik{obj.name}")) scale.y += changeStep;

            // Z ekseni olcek
            ImGui.Text($"Derinlik (Z): {scale.z:F2}");
            if (ImGui.Button($"-##derinlik{obj.name}")) scale.z = Mathf.Max(0.1f, scale.z - changeStep);
            ImGui.SameLine();
            if (ImGui.Button($"+##derinlik{obj.name}")) scale.z += changeStep;

            // Degisikleri uygula
            obj.transform.localScale = scale;

            ImGui.Text($"Eylemsizlik Momenti (I): {inertia:F2}");
            ImGui.Text($"Aci Hiz:                 {angularVel:F2}");
        }

        ImGui.Separator();
        if (ImGui.Button("Kutuyu Durdur"))
        {
            foreach (var obj in objects)
            {
                obj.velocity        = Vector3.zero;
                obj.angularVelocity = Vector3.zero;
            }
        }

        ImGui.End();
    }
}
