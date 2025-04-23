using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using UnityEngine.SceneManagement;

public class BoxCollisionImguı : MonoBehaviour
{
    public GameObject[] boxes;
    public PhysicMaterial[] materials;
    private static GameObject selectedBox;
    private static PhysicMaterial selectedMaterial;

    private List<Rigidbody> boxRigidbodies;
    private static bool isLayoutRegistered = false;

    private float objectSpeed = 0f;
    private float requiredForceToMove = 0f;
    private float dynamicFrictionForce = 0f;
    private float netForce = 0f;
    private const float gravity = 9.81f;

    private Dictionary<GameObject, Vector3> previousVelocities = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> previousAngularVelocities = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        boxRigidbodies = new List<Rigidbody>();

        for (int i = 0; i < Mathf.Min(boxes.Length, materials.Length); i++)
        {
            if (boxes[i] != null && boxes[i].GetComponent<Rigidbody>() != null)
            {
                boxRigidbodies.Add(boxes[i].GetComponent<Rigidbody>());
            }
        }

        if (materials.Length != boxes.Length)
        {
            Debug.LogWarning("Boxes ve Materials dizilerinin uzunlukları aynı değil.");
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
        if (selectedBox != null)
        {
            Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
            if (rb != null)
            {
                objectSpeed = rb.velocity.magnitude;
                netForce = dynamicFrictionForce;
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
                netForce = dynamicFrictionForce;
            }
        }
    }

    void OnLayout()
    {
        if (ImGui.Begin("Fizik Özellikleri"))
        {
            ImGui.Text("Bir Kutu Seçin:");
            ImGui.Separator();

            int count = Mathf.Min(boxes.Length, materials.Length);
            for (int i = 0; i < count; i++)
            {
                GameObject box = boxes[i];
                PhysicMaterial mat = materials[i];

                if (box == null || mat == null)
                    continue;

                if (ImGui.RadioButton($"Seç {box.name}", selectedBox == box))
                {
                    selectedBox = box;
                    selectedMaterial = mat;
                    CalculateFrictionForces();
                }
            }

            // 🟦 Durdurma ve devam ettirme butonları
            if (ImGui.Button("Tüm Kutuları Durdur"))
            {
                StopAllBoxes();
            }

            if (ImGui.Button("Tüm Kutuları Devam Ettir"))
            {
                ResumeAllBoxes();
            }

            // 🔄 Sahneyi yeniden başlatma butonu
            if (ImGui.Button("Sahneyi Yeniden Başlat"))
            {
                RestartScene();
            }

            ImGui.Separator();

            if (selectedBox != null && selectedMaterial != null)
            {
                Rigidbody rb = selectedBox.GetComponent<Rigidbody>();
                ImGui.Text($"Seçili Kutu: {selectedBox.name}");

                float mass = rb.mass;
                if (ImGui.SliderFloat("Kütle", ref mass, 0.1f, 100f))
                {
                    rb.mass = mass;
                    CalculateFrictionForces();
                }

                float staticFriction = selectedMaterial.staticFriction;
                if (ImGui.SliderFloat("Statik Sürtünme", ref staticFriction, 0f, 1f))
                {
                    selectedMaterial.staticFriction = staticFriction;
                    CalculateFrictionForces();
                }

                float dynamicFriction = selectedMaterial.dynamicFriction;
                if (ImGui.SliderFloat("Dinamik Sürtünme", ref dynamicFriction, 0f, 1f))
                {
                    selectedMaterial.dynamicFriction = dynamicFriction;
                    CalculateFrictionForces();
                }

                float bounciness = selectedMaterial.bounciness;
                if (ImGui.SliderFloat("Esneklik (Bounciness)", ref bounciness, 0f, 1f))
                {
                    selectedMaterial.bounciness = bounciness;
                }

                ImGui.Separator();
                ImGui.Text("Gerçek Zamanlı Veriler:");
                ImGui.Text($"Hız: {objectSpeed:F2} m/s");
                ImGui.Text($"Hareket İçin Gerekli Kuvvet: {requiredForceToMove:F2} N");
                ImGui.Text($"Dinamik Sürtünme Kuvveti: {dynamicFrictionForce:F2} N");
                ImGui.Text($"Net Kuvvet: {netForce:F2} N");
            }
        }
        ImGui.End();
    }

    void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void StopAllBoxes()
    {
        previousVelocities.Clear();
        previousAngularVelocities.Clear();

        foreach (GameObject box in boxes)
        {
            if (box != null)
            {
                Rigidbody rb = box.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    previousVelocities[box] = rb.velocity;
                    previousAngularVelocities[box] = rb.angularVelocity;

                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    void ResumeAllBoxes()
    {
        foreach (GameObject box in boxes)
        {
            if (box != null)
            {
                Rigidbody rb = box.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (previousVelocities.ContainsKey(box))
                        rb.velocity = previousVelocities[box];

                    if (previousAngularVelocities.ContainsKey(box))
                        rb.angularVelocity = previousAngularVelocities[box];
                }
            }
        }
    }
}
