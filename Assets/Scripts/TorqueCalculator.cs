using System.Collections.Generic;
using UnityEngine;

public class TorqueCalculator : MonoBehaviour
{
    public Transform pivot;
    public List<MassBlock> blocks = new List<MassBlock>();
    public float gravity = 9.81f;

    void Update()
    {
        if (pivot == null || blocks == null || blocks.Count == 0)
            return; // Hiçbir şey yapma, boş veri

        float leftTorque = 0f;
        float rightTorque = 0f;

        foreach (var block in blocks)
        {
            if (block == null) continue; // Blok yoksa atla

            float distance = block.transform.position.x - pivot.position.x;
            float force = block.blockMass * gravity;
            float torque = Mathf.Abs(distance) * force;

            if (distance < 0)
                leftTorque += torque;
            else
                rightTorque += torque;
        }

        Debug.Log($"Sol Tork: {leftTorque:F2} | Sağ Tork: {rightTorque:F2}");

        if (Mathf.Abs(leftTorque - rightTorque) < 1f)
            Debug.Log("⚖️ DENGEDE");
        else if (leftTorque > rightTorque)
            Debug.Log("↙️ SOL ağır");
        else
            Debug.Log("↘️ SAĞ ağır");
    }
}
