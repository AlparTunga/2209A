using UnityEngine;
using ImGuiNET;

public class DearImGuiDemo : MonoBehaviour
{
    [SerializeField] private PhysicMaterial[] physicMaterials; // Fizik materyallerinin bir listesi
    [SerializeField] private Rigidbody[] balls; // Topların Rigidbody bileşenleri
    [SerializeField] private Vector3 restartForce = new Vector3(0, 10, 0); // Tekrar başlatma kuvveti

    void OnEnable()
    {
        ImGuiUn.Layout += OnLayout;
    }

    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
    }

    void OnLayout()
    {
        // Başlık
        ImGui.Begin("Physic Material Editor");

        // Fizik materyalleri kontrol et
        if (physicMaterials != null && physicMaterials.Length > 0)
        {
            // Her bir fizik materyali için düzenleme alanı
            for (int i = 0; i < physicMaterials.Length; i++)
            {
                PhysicMaterial material = physicMaterials[i];

                ImGui.Text($"Material: {material.name}");
                float bounciness = material.bounciness;

                // Bounciness değerini değiştirmek için kaydırıcı
                if (ImGui.SliderFloat($"Bounciness##{i}", ref bounciness, 0f, 1f))
                {
                    material.bounciness = bounciness;
                }

                // Değeri değiştirmek için buton
                if (ImGui.Button($"Apply##{i}"))
                {
                    material.bounciness = bounciness;
                }

                ImGui.Separator(); // Görsel bir ayırıcı ekle
            }
        }
        else
        {
            ImGui.Text("No Physic Materials available.");
        }

        // Tekrar başlatma butonu
        if (ImGui.Button("Restart Balls"))
        {
            RestartBalls();
        }

        ImGui.End();
    }

    private void RestartBalls()
    {
        if (balls != null && balls.Length > 0)
        {
            foreach (Rigidbody ball in balls)
            {
                ball.velocity = Vector3.zero;
                ball.angularVelocity = Vector3.zero;
                ball.AddForce(restartForce, ForceMode.Impulse);
            }
        }
    }
}