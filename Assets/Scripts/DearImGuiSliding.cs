using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;

public class DearImGuiSliding : MonoBehaviour
{
    public GameObject[] boxes; // Kutuların referansları
    public PhysicMaterial[] materials; // Fizik materyallerinin referansları
    private static GameObject selectedBox; // Şu an seçili olan kutu
    private static PhysicMaterial selectedMaterial; // Seçili materyal
    private static float forceOnBox = 25f; // Varsayılan kuvvet değeri

    private List<Rigidbody> boxRigidbodies; // Kutuların Rigidbody bileşenleri
    private bool applyForce;
    private static bool isLayoutRegistered = false; // Layout'un birden fazla kez kaydedilmesini önlemek için

    private float objectSpeed = 0f; // Kutunun anlık hızı
    private float requiredForceToMove = 0f; // Hareket etmek için gereken minimum kuvvet
    private float dynamicFrictionForce = 0f; // Dinamik sürtünmenin yavaşlatıcı etkisi
    private float netForce = 0f; // Net kuvvet
    private const float gravity = 9.81f; // Yerçekimi ivmesi

    void Start()
    {
        boxRigidbodies = new List<Rigidbody>();
        
        foreach (var box in boxes)
        {
            if (box != null && box.GetComponent<Rigidbody>() != null)
            {
                boxRigidbodies.Add(box.GetComponent<Rigidbody>());
            }
        }
    }

    void OnEnable()
    {
        if (!isLayoutRegistered)
        {
            ImGuiUn.Layout += OnLayout;
            isLayoutRegistered = true;
        }
    }

    void OnDisable()
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
            Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(new Vector3(forceOnBox, 0, 0));
            }
        }

        if (selectedBox != null)
        {
            Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                objectSpeed = rb.velocity.magnitude;
                netForce = forceOnBox - dynamicFrictionForce;
            }
        }
    }

    void CalculateFrictionForces()
    {
        if (selectedMaterial != null && selectedBox != null)
        {
            Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                requiredForceToMove = selectedMaterial.staticFriction * rb.mass * gravity;
                dynamicFrictionForce = selectedMaterial.dynamicFriction * rb.mass * gravity;
                netForce = forceOnBox - dynamicFrictionForce;
            }
        }
    }

    void OnLayout()
    {
        if (ImGui.Begin("Fizik Özellikleri"))
        {
            ImGui.Text("Bir Kutu Seçin:");
            ImGui.Separator();

            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i] != null)
                {
                    if (ImGui.RadioButton($"Seç {boxes[i].name}", selectedBox == boxes[i]))
                    {
                        selectedBox = boxes[i];
                        selectedMaterial = materials[i];
                        CalculateFrictionForces();
                    }
                }
            }

            ImGui.Separator();

            if (selectedBox != null && selectedMaterial != null)
            {
                Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
                ImGui.Text($"Seçili Kutu: {selectedBox.name}");
                ImGui.SliderFloat("Uygulanan Kuvvet", ref forceOnBox, 0f, 100f);
                
                float mass = rb.mass;
                if (ImGui.SliderFloat("Kütle", ref mass, 0.1f, 100f))
                {
                    rb.mass = mass;
                    CalculateFrictionForces();
                }
                
                float drag = rb.drag;
                if (ImGui.SliderFloat("Hava Direnci (Drag)", ref drag, 0f, 5f))
                    rb.drag = drag;
                
                float angularDrag = rb.angularDrag;
                if (ImGui.SliderFloat("Dönme Direnci (Angular Drag)", ref angularDrag, 0f, 5f))
                    rb.angularDrag = angularDrag;
                
                float staticFriction = selectedMaterial.staticFriction;
                if (ImGui.SliderFloat("Statik Sürtünme", ref staticFriction, 0f, 1f))
                {
                    selectedMaterial.staticFriction = staticFriction;
                    materials[System.Array.IndexOf(materials, selectedMaterial)].staticFriction = staticFriction;
                    CalculateFrictionForces();
                }
                
                float dynamicFriction = selectedMaterial.dynamicFriction;
                if (ImGui.SliderFloat("Dinamik Sürtünme", ref dynamicFriction, 0f, 1f))
                {
                    selectedMaterial.dynamicFriction = dynamicFriction;
                    materials[System.Array.IndexOf(materials, selectedMaterial)].dynamicFriction = dynamicFriction;
                    CalculateFrictionForces();
                }

                ImGui.Separator();
                ImGui.Text("Gerçek Zamanlı Veriler:");
                ImGui.Text($"Hız: {objectSpeed:F2} m/s");
                ImGui.Text($"Hareket İçin Gerekli Kuvvet: {requiredForceToMove:F2} N");
                ImGui.Text($"Dinamik Sürtünme Kuvveti (Yavaşlatıcı Etki): {dynamicFrictionForce:F2} N");
                ImGui.Text($"Net Kuvvet: {netForce:F2} N");
                ImGui.Text(forceOnBox >= requiredForceToMove ? "Durum: Hareket Ediyor" : "Durum: Durduruldu");
                
                if (ImGui.Button("Kuvvet Uygula"))
                {
                    applyForce = true;
                }

                if (ImGui.Button("Kuvveti Durdur"))
                {
                    applyForce = false;
                }
            }
        }
        ImGui.End();
    }
}
