using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;

public class DearImGuiSliding : MonoBehaviour
{
    public GameObject[] boxes; // Kutular�n referanslar�
    private static GameObject selectedBox; // �u an se�ili olan kutu
    private static float forceOnBox = 25f; // Varsay�lan kuvvet de�eri

    private Dictionary<GameObject, Rigidbody> boxRigidbodies; // Kutular�n Rigidbody bile�enleri
    private bool applyForce;

    private static bool isLayoutRegistered = false; // Layout'un birden fazla kez kaydedilmesini �nlemek i�in

    void Start()
    {
        // Kutular�n Rigidbody bile�enlerini sakla
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
        // Layout ba�lant�s�n� temizle
        if (isLayoutRegistered)
        {
            ImGuiUn.Layout -= OnLayout;
            isLayoutRegistered = false;
        }
    }

    void FixedUpdate()
    {
        // Kuvvet sadece se�ili kutu varsa ve kuvvet uygulan�yorsa �al���r
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
            ImGui.Separator(); // G�rsel olarak grupland�rmak i�in bir ay�r�c� �izgi

            // Kutular i�in radio button'lar� olu�tur
            foreach (var box in boxes)
            {
                if (box != null)
                {
                    // Her kutu i�in bir radio button
                    if (ImGui.RadioButton($"Select {box.name}", selectedBox == box))
                    {
                        selectedBox = box; // Se�ilen kutuyu g�ncelle
                    }
                }
            }

            ImGui.Separator(); // G�rsel ay�r�c�

            // Sadece bir kutu se�iliyse kontrolleri g�ster
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
