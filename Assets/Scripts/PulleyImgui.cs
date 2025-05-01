using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;

public class PulleyImGuiController : MonoBehaviour
{
    [SerializeField] private PulleySystem pulleySystem;
    [SerializeField] private Transform weight;
    [SerializeField] private LineRenderer ropeRenderer;
    [SerializeField] private Vector3 initialWeightPosition = new Vector3(0, 0.5f, 0);

    private string[] setupNames = { "1:2 Makara", "1:4 Makara", "1:6 Makara", "1:8 Makara" };
    private int currentSetupIndex = 0;

    private List<Transform> allFixed = new List<Transform>();
    private List<Transform> allMoving = new List<Transform>();

    private float weightMass = 10f;
    private float appliedForce = 10f;
    private float netForce = 0f;
    private float requiredForce = 0f;
    private float gravity = 9.81f;
    private float maxHeight = 2f;

    private float currentVelocity = 0f; // fiziksel hız
    private float acceleration = 0f;

    void Start()
    {
        foreach (Transform child in pulleySystem.transform)
        {
            if (child.name.Contains("Fixed"))
                allFixed.Add(child);
        }

        foreach (Transform child in weight)
        {
            if (child.name.Contains("Moving"))
                allMoving.Add(child);
        }

        SwitchToSetup(currentSetupIndex);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (Mathf.Abs(netForce) > 0.1f)
            {
                float acceleration = netForce / weightMass; // a = F/m
                currentVelocity += acceleration * Time.deltaTime;

                Vector3 movement = Vector3.up * currentVelocity * Time.deltaTime;

                // Yukarı çıkışta sınır var
                if (weight.position.y >= maxHeight && movement.y > 0)
                    return;

                weight.Translate(movement);
            }
        }
    }

    void OnEnable() => ImGuiUn.Layout += OnLayout;
    void OnDisable() => ImGuiUn.Layout -= OnLayout;

    void OnLayout()
    {
        ImGui.Begin("Makara Egitimi");

        ImGui.Text("Makara Duzenekleri ve Kuvvet Kazanci");

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
        }

        ImGui.Separator();
        CalculatePhysics();

        ImGui.Text($"Secilen Sistem: {setupNames[currentSetupIndex]}");
        ImGui.Text($"Kuvvet Kazanci (MA): 1:{(currentSetupIndex + 1) * 2}");
        ImGui.Text($"Agirlik Kuvveti (m*g): {(weightMass * gravity):F2} N");
        ImGui.Text($"Gereken Kuvvet: {requiredForce:F2} N");
        ImGui.Text($"Net Kuvvet: {netForce:F2} N");
        ImGui.Text($"Ivme (a): {acceleration:F2} m/s²");
        ImGui.Text($"Hiz (v): {currentVelocity:F2} m/s");

       

        ImGui.Separator();
        if (ImGui.Button("Reset"))
            ResetSystem();

        ImGui.End();
    }

    private void CalculatePhysics()
    {
        float ma = (currentSetupIndex + 1) * 2f; // Makara kazancı: 1:2, 1:4, 1:6, 1:8
        requiredForce = (weightMass * gravity) / ma;
        netForce = appliedForce - requiredForce;
        
        // İvme hesapla
        acceleration = netForce / weightMass;
    }

    private void SwitchToSetup(int level)
    {
        List<Transform> activePulleys = new List<Transform>();

        foreach (var p in allFixed) p.gameObject.SetActive(false);
        foreach (var p in allMoving) p.gameObject.SetActive(false);

        for (int i = 0; i <= level; i++)
        {
            if (i < allMoving.Count)
            {
                allMoving[i].gameObject.SetActive(true);
                activePulleys.Add(allMoving[i]);
            }

            if (i < allFixed.Count)
            {
                allFixed[i].gameObject.SetActive(true);
                activePulleys.Add(allFixed[i]);
            }
        }

        pulleySystem.weight = weight;
        pulleySystem.ropeRenderer = ropeRenderer;
        pulleySystem.pulleyPoints = activePulleys.ToArray();
    }

    private void ResetSystem()
    {
        weight.position = initialWeightPosition;
        currentVelocity = 0f; // Hızı sıfırla
        SwitchToSetup(currentSetupIndex);
    }
}
