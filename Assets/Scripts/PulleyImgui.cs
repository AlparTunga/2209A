using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;

public class PulleyImGuiController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PulleySystem pulleySystem;
    [SerializeField] private Transform weight;
    [SerializeField] private LineRenderer ropeRenderer;
    [SerializeField] private Vector3 initialWeightPosition = new Vector3(0, 0.5f, 0);

    private string[] setupNames = { "1:2 Makara", "1:4 Makara", "1:6 Makara", "1:8 Makara" };
    private int currentSetupIndex = 0;

    private List<Transform> allFixed = new List<Transform>();
    private List<Transform> allMoving = new List<Transform>();

    private float weightMass    = 10f;
    private float appliedForce  = 10f;
    private float netForce      = 0f;
    private float requiredForce = 0f;
    private float gravity       = 9.81f;
    private float maxHeight     = 2f;

    private float currentVelocity = 0f;
    private float acceleration    = 0f;

    // Intro ekran flag’i
    private bool showIntro = true;

    void Start()
    {
        // Sabit ve hareketli makaralari listele
        foreach (Transform child in pulleySystem.transform)
            if (child.name.Contains("Fixed"))
                allFixed.Add(child);

        foreach (Transform child in weight)
            if (child.name.Contains("Moving"))
                allMoving.Add(child);

        // Baslangicta ilgili duzeni ayarla
        SwitchToSetup(currentSetupIndex);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Mathf.Abs(netForce) > 0.1f)
        {
            acceleration     = netForce / weightMass;
            currentVelocity += acceleration * Time.deltaTime;

            Vector3 move = Vector3.up * currentVelocity * Time.deltaTime;
            if (weight.position.y >= maxHeight && move.y > 0)
                return;

            weight.Translate(move);
        }
    }

    void OnEnable()  => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        // Pencereyi icerige gore otomatik yeniden boyutla
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Makara Sistemleri ve Kuvvet Kazanci - Giris", flags);

            ImGui.TextWrapped("Makara sistemleri yukari cikmak icin kullanilir ve kuvvet kazanci saglar.");
            ImGui.TextWrapped("Sabit ve hareketli makaralarin sayisina gore kuvvet kazanci hesaplanir.");

            ImGui.BulletText("MA        = F_cikti / F_girdi    (Mekanik avantaj)");
            ImGui.BulletText("F_gerekli = (m * g) / MA         (Gereken kuvvet)");
            ImGui.BulletText("a         = F_net / m            (Ivme)");

            ImGui.Separator();
            if (ImGui.Button("Continue"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        ImGui.Begin("Makara Egitimi", flags);

        ImGui.Text("Makara duzenekleri ve kuvvet kazanci");
        ImGui.SliderFloat("Agirlik (kg)", ref weightMass, 1f, 100f);
        ImGui.SliderFloat("Uygulanan Kuvvet (N)", ref appliedForce, 1f, 200f);

        ImGui.Separator();
        for (int i = 0; i < setupNames.Length; i++)
        {
            if (ImGui.Button(setupNames[i]))
            {
                SwitchToSetup(i);
                currentSetupIndex = i;
            }
            if (i < setupNames.Length - 1)
                ImGui.SameLine();
        }

        ImGui.Separator();
        CalculatePhysics();

        ImGui.Text($"Secilen Sistem:      {setupNames[currentSetupIndex]}");
        ImGui.Text($"Kuvvet Kazanci:      1:{(currentSetupIndex + 1) * 2}");
        ImGui.Text($"Agirlik Kuvveti:     {(weightMass * gravity):F2} N");
        ImGui.Text($"Gereken Kuvvet:      {requiredForce:F2} N");
        ImGui.Text($"Net Kuvvet:          {netForce:F2} N");
        ImGui.Text($"Ivme (a):            {acceleration:F2} m/s2");
        ImGui.Text($"Hiz (v):             {currentVelocity:F2} m/s");

        ImGui.Separator();
        if (ImGui.Button("Reset"))
            ResetSystem();

        ImGui.End();
    }

    private void CalculatePhysics()
    {
        float ma = (currentSetupIndex + 1) * 2f; // 1:2, 1:4, 1:6, 1:8
        requiredForce = (weightMass * gravity) / ma;
        netForce      = appliedForce - requiredForce;
    }

    private void SwitchToSetup(int level)
    {
        // Sabit ve hareketli makaralari kapat
        foreach (var t in allFixed)  t.gameObject.SetActive(false);
        foreach (var t in allMoving) t.gameObject.SetActive(false);

        List<Transform> active = new List<Transform>();
        for (int i = 0; i <= level; i++)
        {
            if (i < allMoving.Count)
            {
                allMoving[i].gameObject.SetActive(true);
                active.Add(allMoving[i]);
            }
            if (i < allFixed.Count)
            {
                allFixed[i].gameObject.SetActive(true);
                active.Add(allFixed[i]);
            }
        }

        pulleySystem.weight       = weight;
        pulleySystem.ropeRenderer = ropeRenderer;
        pulleySystem.pulleyPoints = active.ToArray();
    }

    private void ResetSystem()
    {
        weight.position    = initialWeightPosition;
        currentVelocity    = 0f;
        netForce           = 0f;
        acceleration       = 0f;
        SwitchToSetup(currentSetupIndex);
    }
}
