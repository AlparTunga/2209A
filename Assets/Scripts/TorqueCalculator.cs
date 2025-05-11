using System.Collections.Generic;
using UnityEngine;

public class TorqueCalculator : MonoBehaviour
{
    [Header("Seesaw ve Bloklar")]
    [Tooltip("Dönen seesaw objesinin Transform'u")]
    public Transform seesawTransform;
    [Tooltip("Listeye sahnede yer alan MassBlock referanslarını ekleyin")]
    public List<MassBlock> blocks = new List<MassBlock>();

    [Header("Fizik Ayarları")]
    public float gravity = 9.81f;
    [Tooltip("Nm cinsinden tork farkı toleransı (dengede saymak için)")]
    public float balanceTolerance = 1f;

    void Update()
    {
        if (seesawTransform == null || blocks == null || blocks.Count == 0)
            return;

        float leftTorque = 0f;
        float rightTorque = 0f;

        foreach (var block in blocks)
        {
            // Boş veya henüz slot'a yerleşmemiş blokları atla
            if (block == null || !block.HasBeenPlaced())
                continue;

            // Seesaw'ın LOCAL X eksenindeki lever‐arm
            float dx = seesawTransform.InverseTransformPoint(block.transform.position).x;
            float force = block.blockMass * gravity;
            float torque = Mathf.Abs(dx) * force;

            if (dx < 0f) leftTorque += torque;
            else rightTorque += torque;
        }

        // Konsola yazdırmak istersen:
        Debug.Log($"Sol Tork: {leftTorque:F2} Nm | Sağ Tork: {rightTorque:F2} Nm");

        float diff = Mathf.Abs(leftTorque - rightTorque);

        if (diff <= balanceTolerance)
        {
            Debug.Log("⚖️ DENGEDE");
        }
        else if (leftTorque > rightTorque)
        {
            Debug.Log("↙️ SOL ağır");
        }
        else
        {
            Debug.Log("↘️ SAĞ ağır");
        }
    }
}
