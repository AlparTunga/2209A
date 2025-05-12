using UnityEngine;
using ImGuiNET;
using System.Linq;

public class TagOfWarImGui : MonoBehaviour
{
    [Header("Drag your TugOfWarManager here")]
    public TugOfWarManager manager;

    readonly float[] presets = { 50f, 100f, 150f, 200f };
    private int selectedPreset = 0;
    private Vector2 scrollPos;

    // --- GIRIS EKRANI FLAG'I ---
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
        if (manager == null)
            return;

        // Pencereyi icerige gore otomatik boyutla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Tug-Of-War - Giris", flags);

            ImGui.TextWrapped("Tug-of-war oyununda iki taraf ipin iki ucundan cekerek kazanmaya calisir.");
            ImGui.TextWrapped("Her cekici belirli bir kuvvet uygular, surutme kuvveti de yonu degistirir.");

            ImGui.BulletText("F_left      = Sol cekicilerin toplam kuvveti");
            ImGui.BulletText("F_right     = Sag cekicilerin toplam kuvveti");
            ImGui.BulletText("F_friction = -drag * hiz");
            ImGui.BulletText("F_net       = F_left - F_right + F_friction");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        ImGui.Begin("Tug-Of-War Kontrolleri", flags);

        // 1) Force Preset
        ImGui.Text("Kuvvet Preseti:");
        for (int i = 0; i < presets.Length; i++)
        {
            if (ImGui.Button($"{presets[i]:F0} N"))
                selectedPreset = i;
            if (i < presets.Length - 1)
                ImGui.SameLine();
        }

        ImGui.Separator();

        // 2) Sol / Sag Ekle
        if (ImGui.Button($"Sol Ekle   ({presets[selectedPreset]:F0} N)"))
            manager.AddLeftPuller(presets[selectedPreset]);
        ImGui.SameLine();
        if (ImGui.Button($"Sag Ekle   ({presets[selectedPreset]:F0} N)"))
            manager.AddRightPuller(presets[selectedPreset]);

        ImGui.Separator();

        // 3) Baslat / Durdur / Sifirla
        if (ImGui.Button("Yarisi Baslat"))  manager.StartRace();
        ImGui.SameLine();
        if (ImGui.Button("Yarisi Durdur"))  manager.StopRace();
        ImGui.SameLine();
        if (ImGui.Button("Yarisi Sifirla")) manager.ResetRace();

        ImGui.Separator();

        // 4) Cekiciler Listesi
        ImGui.Text("Cekiciler:");
        ImGui.BeginChild("CekicilerRegion", new Vector2(300, 200), true);
        manager.pullers.RemoveAll(p => p == null);
        foreach (var item in manager.pullers.Select((puller, idx) => (puller, idx)))
        {
            float dir  = Mathf.Sign(manager.wagonRb.position.x - item.puller.transform.position.x);
            string side = dir > 0 ? "L" : "R";
            bool pulling = item.puller.IsPulling;

            ImGui.Text($"{item.idx + 1}. {side}  {item.puller.pullForce:F0} N  {(pulling ? "🔴" : "⚪")}");
            ImGui.SameLine();
            if (ImGui.Button($"Kaldir##{item.idx}"))
            {
                manager.pullers.RemoveAt(item.idx);
                Destroy(item.puller.gameObject);
            }
        }
        ImGui.EndChild();

        ImGui.Separator();

        // 5) Kuvvet Hesaplamalari
        float leftSum  = manager.pullers
            .Where(p => p.IsPulling && p.transform.position.x < manager.wagonRb.position.x)
            .Sum(p => p.pullForce);
        float rightSum = manager.pullers
            .Where(p => p.IsPulling && p.transform.position.x > manager.wagonRb.position.x)
            .Sum(p => p.pullForce);
        float frictionFx = -manager.wagonRb.drag * manager.wagonRb.velocity.x;
        float net = leftSum - rightSum + frictionFx;

        ImGui.Text($"Sol Toplam:    {leftSum:F1} N");
        ImGui.Text($"Sag Toplam:    {rightSum:F1} N");
        ImGui.Text($"Surutme:       {frictionFx:F1} N");
        ImGui.Separator();
        ImGui.Text($"Net Kuvvet:    {net:F1} N");

        ImGui.End();
    }
}
