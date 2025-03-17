using ImGuiNET;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DearImGuiMassEditor : MonoBehaviour
{
    public GameObject[] objectsToEdit; // Rigidbody bileşenlerine sahip nesneler
    private float[] masses; // Mass değerlerini saklamak için dizi
    private static float[] savedMasses; // Değiştirilen mass değerlerini saklamak için static dizi
    private bool isGamePlaying = false; // Oyun durumu kontrolü

    private void Start()
    {
        if (objectsToEdit != null && objectsToEdit.Length > 0)
        {
            if (savedMasses == null || savedMasses.Length != objectsToEdit.Length)
            {
                savedMasses = new float[objectsToEdit.Length];
                for (int i = 0; i < objectsToEdit.Length; i++)
                {
                    Rigidbody rb = objectsToEdit[i].GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        savedMasses[i] = rb.mass;
                    }
                }
            }

            masses = new float[objectsToEdit.Length];
            for (int i = 0; i < objectsToEdit.Length; i++)
            {
                masses[i] = savedMasses[i];
                Rigidbody rb = objectsToEdit[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = masses[i];
                }
            }
        }
    }

    private void OnEnable() => ImGuiUn.Layout += OnLayout;
    private void OnDisable() => ImGuiUn.Layout -= OnLayout;

    private void Update()
    {
        if (!isGamePlaying)
        {
            Time.timeScale = 0; // Oyun durur
        }
        else
        {
            Time.timeScale = 1; // Oyun çalışır
        }
    }

    private void OnLayout()
    {
        ImGui.SetNextWindowSize(new UnityEngine.Vector2(500, 400), ImGuiCond.FirstUseEver);
        
        if (!ImGui.Begin("Rigidbody Mass Editor"))
        {
            ImGui.End();
            return;
        }

        if (!isGamePlaying)
        {
            if (ImGui.Button("Play"))
            {
                isGamePlaying = true; // Play tuşuna basınca oyun başlar
            }
        }

        for (int i = 0; i < objectsToEdit.Length; i++)
        {
            if (objectsToEdit[i] != null)
            {
                Rigidbody rb = objectsToEdit[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ImGui.Text($"Object {i + 1}: {objectsToEdit[i].name}");
                    ImGui.PushItemWidth(150);
                    if (ImGui.SliderFloat($"##MassSlider{i}", ref masses[i], 0.1f, 100f))
                    {
                        rb.mass = masses[i];
                    }
                    ImGui.SameLine();
                    ImGui.PushItemWidth(50);
                    if (ImGui.InputFloat($"##MassInput{i}", ref masses[i]))
                    {
                        rb.mass = masses[i];
                    }
                    ImGui.PopItemWidth();

                    // Açıyı hesapla ve göster
                    Vector3 velocity = rb.velocity;
                    float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                    ImGui.Text($"Launch Angle: {angle:F2} degrees");
                }
                else
                {
                    ImGui.Text($"Object {i + 1} has no Rigidbody component.");
                }
            }
            else
            {
                ImGui.Text($"Object {i + 1} is null.");
            }
        }

        if (ImGui.Button("Restart Game"))
        {
            for (int i = 0; i < objectsToEdit.Length; i++)
            {
                if (objectsToEdit[i] != null)
                {
                    Rigidbody rb = objectsToEdit[i].GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        savedMasses[i] = masses[i];
                    }
                }
            }
            isGamePlaying = false; // Restart sonrası oyun durur
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        ImGui.End();
    }
}