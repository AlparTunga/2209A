using UnityEngine;
using ImGuiNET;

public class TorqueImGuiUI : MonoBehaviour
{
    [Header("Spawner & Slotlar")]
    public WeightSpawner weightSpawner;
    public Transform[] slotTransforms;

    [Header("Torque Hesabi")]
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

        if (!introComplete)
        {
            Vector2 introSize = new Vector2(480, 240);
            Vector2 introPos = (screenSize - introSize) * 0.5f;
            ImGui.SetNextWindowSize(introSize, ImGuiCond.Always);
            ImGui.SetNextWindowPos(introPos, ImGuiCond.Always);

            introAlpha = Mathf.Clamp01(introAlpha + Time.deltaTime * introFadeSpeed);
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, introAlpha);

            ImGui.Begin("Torque Girisi", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove);

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

        ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.FirstUseEver);
        ImGui.Begin("Oyun Kontrolleri", ImGuiWindowFlags.AlwaysAutoResize);

        if (ImGui.Button("Spawn Block"))
            weightSpawner.SpawnBlock();

        float boardLength = seesawRenderer.GetComponent<MeshFilter>().sharedMesh.bounds.size.x * seesawTransform.localScale.x;
        ImGui.Text($"Tahta Uzunlugu: {boardLength:F2} m");

        ImGui.Text("Slotlar merkeze uzakliklari:");
        foreach (var slot in slotTransforms)
        {
            float dx = Mathf.Abs(seesawTransform.InverseTransformPoint(slot.position).x);
            ImGui.Text($"{slot.name}: {dx:F2} m");
        }

        var current = WeightSpawner.currentBlock;
        if (current != null)
        {
            ImGui.Separator();
            ImGui.Text($"Secili: {current.gameObject.name}");
            ImGui.Separator();

            foreach (var slot in slotTransforms)
            {
                bool occ = false;
                foreach (var b in FindObjectsOfType<MassBlock>())
                    if (b.HasBeenPlaced() && b.transform.parent == slot)
                    { occ = true; break; }
            }
        }
        ImGui.End();

        ImGui.SetNextWindowPos(new Vector2(10, 400), ImGuiCond.Always);
        ImGui.Begin("Tork Denklemi", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
        ImGui.Text("Tork = Kuvvet x Kol uzunlugu");
        ImGui.End();

        ImGui.SetNextWindowPos(new Vector2(screenSize.x - 10, 10), ImGuiCond.FirstUseEver, new Vector2(1f, 0f));
        ImGui.Begin("Torque Bilgileri", ImGuiWindowFlags.AlwaysAutoResize);

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

        // Kuvvet Formulu Penceresi (sol alt daha asagida)
        ImGui.SetNextWindowPos(new Vector2(screenSize.x - 10, screenSize.y - 10), ImGuiCond.Always, new Vector2(1f, 1f));
        ImGui.Begin("Kuvvet Formulu", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
        ImGui.Text("Kuvvet = Yercekimi=9,81 x Agirlik");
        ImGui.End();
    }
}
