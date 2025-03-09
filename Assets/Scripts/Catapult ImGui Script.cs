using ImGuiNET;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DearImGuiMassEditor : MonoBehaviour
{
    public GameObject[] objectsToEdit; // Rigidbody bileşenlerine sahip nesneler
    private float[] masses; // Mass değerlerini saklamak için dizi

    private static float[] savedMasses; // Değiştirilen mass değerlerini saklamak için static dizi

    private bool isInteractingWithImGui = false; // ImGui ile etkileşim kontrolü

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
        // Eğer ImGui ile etkileşim varsa oyunu durdur, aksi takdirde devam ettir
        if (isInteractingWithImGui)
        {
            Time.timeScale = 0;
        }
        else if (Input.GetMouseButtonDown(0)) // Sadece oyun ekranına tıklanırsa devam et
        {
            Time.timeScale = 1;
        }
    }

    private void OnLayout()
    {
        isInteractingWithImGui = true; // ImGui etkileşimi başladığında oyun durur

        if (!ImGui.Begin("Rigidbody Mass Editor"))
        {
            isInteractingWithImGui = false;
            return;
        }

        for (int i = 0; i < objectsToEdit.Length; i++)
        {
            if (objectsToEdit[i] != null)
            {
                Rigidbody rb = objectsToEdit[i].GetComponent<Rigidbody>();

                if (rb != null)
                {
                    ImGui.Text($"Object {i + 1}: {objectsToEdit[i].name}");
                    if (ImGui.SliderFloat($"Mass {i + 1}", ref masses[i], 0.1f, 100f))
                    {
                        rb.mass = masses[i];
                        savedMasses[i] = masses[i];
                    }
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        ImGui.End();

        isInteractingWithImGui = false; // ImGui etkileşimi bittiğinde oyun devam eder
    }
}
