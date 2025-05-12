using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;

public class DearImGuiSliding : MonoBehaviour
{
    [Header("References")]
    public GameObject[] boxes;              
    public PhysicMaterial[] materials;      

    private static GameObject selectedBox;          
    private static PhysicMaterial selectedMaterial;
    private static float forceOnBox = 25f;          

    private Vector3[] initialPositions;
    private float[] initialMasses, initialDrags, initialAngularDrags;
    private float[] initialStaticFrictions, initialDynamicFrictions;
    private List<Rigidbody> boxRigidbodies = new List<Rigidbody>();
    private bool applyForce;

    private float objectSpeed, requiredForceToMove, dynamicFrictionForce, netForce;
    private static bool isLayoutRegistered = false;

    private bool showIntro = true;

    void Start()
    {
        int count = boxes.Length;
        initialPositions        = new Vector3[count];
        initialMasses           = new float[count];
        initialDrags            = new float[count];
        initialAngularDrags     = new float[count];
        initialStaticFrictions  = new float[count];
        initialDynamicFrictions = new float[count];

        for (int i = 0; i < count; i++)
        {
            if (boxes[i] != null)
            {
                initialPositions[i] = boxes[i].transform.position;
                var rb = boxes[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    boxRigidbodies.Add(rb);
                    initialMasses[i]       = rb.mass;
                    initialDrags[i]        = rb.drag;
                    initialAngularDrags[i] = rb.angularDrag;
                }
                initialStaticFrictions[i]  = materials[i].staticFriction;
                initialDynamicFrictions[i] = materials[i].dynamicFriction;
            }
        }

        if (!isLayoutRegistered)
        {
            ImGuiUn.Layout += OnLayout;
            isLayoutRegistered = true;
        }
    }

    void OnDestroy()
    {
        if (isLayoutRegistered)
        {
            ImGuiUn.Layout -= OnLayout;
            isLayoutRegistered = false;
        }
    }

    void FixedUpdate()
    {
        if (applyForce && selectedBox != null)
        {
            var rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(new Vector3(forceOnBox, 0, 0));
        }
        if (selectedBox != null)
        {
            var rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
                objectSpeed = rb.velocity.magnitude;
        }
    }

    void CalculateFrictionForces()
    {
        if (selectedMaterial != null && selectedBox != null)
        {
            var rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                requiredForceToMove  = selectedMaterial.staticFriction  * rb.mass * Physics.gravity.magnitude;
                dynamicFrictionForce = selectedMaterial.dynamicFriction * rb.mass * Physics.gravity.magnitude;
                netForce             = forceOnBox - dynamicFrictionForce;
            }
        }
    }

    void ResetSelectedBox()
    {
        if (selectedBox == null) return;
        int index = System.Array.IndexOf(boxes, selectedBox);
        if (index < 0) return;

        selectedBox.transform.position = initialPositions[index];
        var rb = selectedBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity         = Vector3.zero;
            rb.angularVelocity  = Vector3.zero;
            rb.mass             = initialMasses[index];
            rb.drag             = initialDrags[index];
            rb.angularDrag      = initialAngularDrags[index];
        }

        materials[index].staticFriction  = initialStaticFrictions[index];
        materials[index].dynamicFriction = initialDynamicFrictions[index];

        forceOnBox = 25f;
        applyForce = false;
        CalculateFrictionForces();
    }

    void OnLayout()
    {
        ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysAutoResize;

        // --- GIRIS EKRANI ---
        if (showIntro)
        {
            ImGui.Begin("Surutme ve Hareket - Giris", flags);

            ImGui.TextWrapped("Bir cismin harekete gecmesi icin statik surutme yenilmelidir.");
            ImGui.TextWrapped("Hareket halindeyken dinamik surutme cisme etki eder.");

            ImGui.BulletText("F_statik  = mu_s * m * g  (Statik surutme)");
            ImGui.BulletText("F_dinamik = mu_k * m * g  (Dinamik surutme)");
            ImGui.BulletText("F_net     = F_uygulanan - F_dinamik");

            ImGui.Separator();
            if (ImGui.Button("Devam Et"))
                showIntro = false;

            ImGui.End();
            return;
        }

        // --- ANA PANEL ---
        ImGui.Begin("Fizik Ozellikleri", flags);

        ImGui.Text("Bir Kutu Secin:");
        ImGui.Separator();

        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] != null && materials[i] != null)
            {
                if (ImGui.RadioButton($"Sec {boxes[i].name}", selectedBox == boxes[i]))
                {
                    selectedBox     = boxes[i];
                    selectedMaterial= materials[i];
                    CalculateFrictionForces();
                }
            }
        }

        ImGui.Separator();
        if (selectedBox != null && selectedMaterial != null)
        {
            var rb = selectedBox.GetComponent<Rigidbody>();

            ImGui.Text($"Secili Kutu: {selectedBox.name}");
            ImGui.SliderFloat("Uygulanan Kuvvet", ref forceOnBox, 0f, 100f);

            float massLocal = rb.mass;
            if (ImGui.SliderFloat("Kutle", ref massLocal, 0.1f, 100f))
            {
                rb.mass = massLocal;
                CalculateFrictionForces();
            }

            float dragLocal = rb.drag;
            if (ImGui.SliderFloat("Hava Direnci (Drag)", ref dragLocal, 0f, 5f))
                rb.drag = dragLocal;

            float angularDragLocal = rb.angularDrag;
            if (ImGui.SliderFloat("Aci Direnci (Angular Drag)", ref angularDragLocal, 0f, 5f))
                rb.angularDrag = angularDragLocal;

            float staticFrictionLocal = selectedMaterial.staticFriction;
            if (ImGui.SliderFloat("Statik Surutme", ref staticFrictionLocal, 0f, 1f))
            {
                selectedMaterial.staticFriction = staticFrictionLocal;
                materials[System.Array.IndexOf(materials, selectedMaterial)].staticFriction = staticFrictionLocal;
                CalculateFrictionForces();
            }

            float dynamicFrictionLocal = selectedMaterial.dynamicFriction;
            if (ImGui.SliderFloat("Dinamik Surutme", ref dynamicFrictionLocal, 0f, 1f))
            {
                selectedMaterial.dynamicFriction = dynamicFrictionLocal;
                materials[System.Array.IndexOf(materials, selectedMaterial)].dynamicFriction = dynamicFrictionLocal;
                CalculateFrictionForces();
            }

            ImGui.Separator();
            ImGui.Text("Gercek Zamanli Veriler:");
            ImGui.Text($"Hiz:                     {objectSpeed:F2} m/s");
            ImGui.Text($"Gerekli Kuvvet:          {requiredForceToMove:F2} N");
            ImGui.Text($"Dinamik Surutme Kuvveti: {dynamicFrictionForce:F2} N");
            ImGui.Text($"Net Kuvvet:              {netForce:F2} N");

            if (ImGui.Button("Kuvveti Uygula"))
                applyForce = true;
            ImGui.SameLine();
            if (ImGui.Button("Kuvveti Durdur"))
                applyForce = false;
            ImGui.SameLine();
            if (ImGui.Button("Sifirla"))
                ResetSelectedBox();
        }

        ImGui.End();
    }
}
