using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;

public class DearImGuiSliding : MonoBehaviour
{
    public GameObject[] boxes; // Kutuların referansları
    private static GameObject selectedBox; // Şu an seçili olan kutu
    private static float forceOnBox = 25f; // Varsayılan kuvvet değeri

    private Dictionary<GameObject, Rigidbody> boxRigidbodies; // Kutuların Rigidbody bileşenleri
    private bool applyForce;

    private static bool isLayoutRegistered = false; // Layout'un birden fazla kez kaydedilmesini önlemek için

    void Start()
    {
        // Kutuların Rigidbody bileşenlerini sakla
        boxRigidbodies = new Dictionary<GameObject, Rigidbody>();
        foreach (var box in boxes)
        {
            if (box != null && box.GetComponent<Rigidbody>() != null)
            {
                boxRigidbodies[box] = box.GetComponent<Rigidbody>();
            }
        }
    }

    void OnEnable()
    {
        // Layout bir kere kaydedilsin
        if (!isLayoutRegistered)
        {
            ImGuiUn.Layout += OnLayout;
            isLayoutRegistered = true;
        }
    }

    void OnDisable()
    {
        // Layout bağlantısını temizle
        if (isLayoutRegistered)
        {
            ImGuiUn.Layout -= OnLayout;
            isLayoutRegistered = false;
        }
    }

    void FixedUpdate()
    {
        // Kuvvet sadece seçili kutu varsa ve kuvvet uygulanıyorsa çalışır
        if (applyForce && selectedBox != null && boxRigidbodies.ContainsKey(selectedBox))
        {
            boxRigidbodies[selectedBox].AddForce(new Vector3(forceOnBox, 0, 0));
        }
    }

    void OnLayout()
    {
        if (ImGui.Begin("Force Control"))
        {
            ImGui.Text("Select a Box:");
            ImGui.Separator(); // Görsel olarak gruplandırmak için bir ayırıcı çizgi

            // Kutular için radio button'ları oluştur
            foreach (var box in boxes)
            {
                if (box != null)
                {
                    // Her kutu için bir radio button
                    if (ImGui.RadioButton($"Select {box.name}", selectedBox == box))
                    {
                        selectedBox = box; // Seçilen kutuyu güncelle
                    }
                }
            }

            ImGui.Separator(); // Görsel ayırıcı

            // Sadece bir kutu seçiliyse kontrolleri göster
            if (selectedBox != null)
            {
                ImGui.Text($"Selected Box: {selectedBox.name}");
                ImGui.SliderFloat("Force", ref forceOnBox, 0f, 100f);

                if (ImGui.Button("Apply Force"))
                {
                    applyForce = true;
                }

                if (ImGui.Button("Stop Force"))
                {
                    applyForce = false;
                }
            }
        }
        ImGui.End();
    }
}
