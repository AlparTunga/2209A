using UnityEngine;
using ImGuiNET;

public class TorqueImGuiUI : MonoBehaviour
{
    [Header("Spawner & Slot’lar")]
    public WeightSpawner weightSpawner;
    public Transform[] slotTransforms;    // Slot_-3 … Slot_+3

    [Header("Torque Hesabı")]
    public Transform seesawTransform;
    public Renderer seesawRenderer;
    public float gravity = 9.81f;

    private bool introComplete = false;
    private float introAlpha = 0f;
    private float introFadeSpeed = 0.5f;

    void OnEnable() => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        var io = ImGui.GetIO();
        Vector2 screenSize = io.DisplaySize;

        // ▶ Intro Ekranı
        if (!introComplete)
        {
            Vector2 introSize = new Vector2(480, 240);
            Vector2 introPos = (screenSize - introSize) * 0.5f;
            ImGui.SetNextWindowSize(introSize, ImGuiCond.Always);
            ImGui.SetNextWindowPos(introPos, ImGuiCond.Always);

            introAlpha = Mathf.Clamp01(introAlpha + Time.deltaTime * introFadeSpeed);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, introAlpha);

            ImGui.Begin("Torque Girişi", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);

            ImGui.TextWrapped("Bu sahne, tahterevalli uzerinde agirliklarin tork etkisini anlaman icin hazirlandi.");
            ImGui.Spacing();
            ImGui.TextWrapped("Fizik formulu:");
            ImGui.Text("Tork = Kuvvet x Kol uzunlugu");
            ImGui.Spacing();
            ImGui.TextWrapped("Bloklari farkli mesafelere yerlestirerek dengenin nasil degistigini gozlemleyebilirsin.");
            ImGui.Spacing();

            if (introAlpha >= 1f && ImGui.Button("Devam Et"))
                introComplete = true;

            ImGui.End();
            ImGui.PopStyleVar();
            return;
        }

        // ◀ Oyun Kontrolleri (sol üst)
        ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.FirstUseEver);
        ImGui.Begin("Oyun Kontrolleri");

        if (ImGui.Button("Spawn Block"))
            weightSpawner.SpawnBlock();

        var current = WeightSpawner.currentBlock;
        if (current != null)
        {
            ImGui.Text($"Seçili: {current.gameObject.name}");
            ImGui.Separator();

            foreach (var slot in slotTransforms)
            {
                bool occ = false;
                foreach (var b in FindObjectsOfType<MassBlock>())
                    if (b.HasBeenPlaced() && b.transform.parent == slot)
                    { occ = true; break; }

                if (!occ && ImGui.Button($"Place on {slot.name}"))
                    WeightSpawner.PlaceCurrentBlock(slot);
                else
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                    ImGui.Button($"Place on {slot.name}");
                    ImGui.PopStyleVar();
                }
            }
        }
        ImGui.End();

        // ▶ Denklem Paneli (sol alt)
        ImGui.SetNextWindowPos(new Vector2(10, 250), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(250, 60), ImGuiCond.Always);
        ImGui.Begin("Tork Denklemi", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
        ImGui.Text("Tork = Kuvvet x Kol uzunlugu");
        ImGui.End();

        // ▶ Torque Bilgileri (sağ üst)
        ImGui.SetNextWindowPos(new Vector2(screenSize.x - 10, 10), ImGuiCond.FirstUseEver, new Vector2(1f, 0f));
        ImGui.Begin("Torque Bilgileri");

        float left = 0f, right = 0f;
        foreach (var b in FindObjectsOfType<MassBlock>())
        {
            if (!b.HasBeenPlaced()) continue;
            float dx = seesawTransform.InverseTransformPoint(b.transform.position).x;
            float f = b.blockMass * gravity;
            float tq = Mathf.Abs(dx) * f;
            if (dx < 0) left += tq; else right += tq;
        }

        float diff = Mathf.Abs(left - right);
        ImGui.Text($"Left Torque:   {left:F2} Nm");
        ImGui.Text($"Right Torque:  {right:F2} Nm");
        ImGui.Text($"Difference:    {diff:F2} Nm");

        ImGui.End();
    }
}
