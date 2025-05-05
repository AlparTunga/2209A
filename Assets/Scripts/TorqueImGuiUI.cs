using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;

public class TorqueImGuiUI : MonoBehaviour
{
    [Header("Torque Hesabý için")]
    [SerializeField] private Transform pivot;
    [SerializeField] private float gravity = 9.81f;

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

        ImGui.Separator();
        ImGui.Text("Torque Bilgileri");

        // --- Torque Hesaplamasý ---
        float leftTorque = 0f;
        float rightTorque = 0f;

        MassBlock[] blocks = FindObjectsOfType<MassBlock>();
        foreach (MassBlock block in blocks)
        {
            if (block == null) continue;

            float distance = block.transform.position.x - pivot.position.x;
            float force = block.blockMass * gravity;
            float torque = Mathf.Abs(distance) * force;

            if (distance < 0)
                leftTorque += torque;
            else
                rightTorque += torque;
        }

        float diff = Mathf.Abs(leftTorque - rightTorque);

        ImGui.Text($"Left Torque:  {leftTorque:F2} Nm");
        ImGui.Text($"Right Torque: {rightTorque:F2} Nm");
        ImGui.Text($"Fark:         {diff:F2} Nm");

        ImGui.End();
    }


}
