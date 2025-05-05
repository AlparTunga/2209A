using System.Collections.Generic;
using UnityEngine;

public class TorqueVisualizer : MonoBehaviour
{
    public Transform pivot;                     // Tahterevallinin ortasý
    public Renderer seesawRenderer;              // Tahterevalli görseli
    public float gravity = 9.81f;                // Yerçekimi
    public float balanceTolerance = 1f;          // Denge toleransý

    private List<MassBlock> blocks = new List<MassBlock>();

    void Update()
    {
        if (pivot == null || seesawRenderer == null)
            return; // Atla, hata vermesin

        RefreshBlocks(); // Bloklarý sahnede otomatik güncelle

        if (blocks.Count == 0)
            return;

        float leftTorque = 0f;
        float rightTorque = 0f;

        foreach (var block in blocks)
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

        float torqueDifference = Mathf.Abs(leftTorque - rightTorque);

        if (torqueDifference <= balanceTolerance)
        {
            seesawRenderer.material.color = Color.green; // Dengede
        }
        else if (leftTorque > rightTorque)
        {
            seesawRenderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01(torqueDifference / 100f)); // Sol aðýr
        }
        else
        {
            seesawRenderer.material.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01(torqueDifference / 100f)); // Sað aðýr
        }
    }

    void RefreshBlocks()
    {
        // Sahnede var olan tüm MassBlock'larý otomatik bul
        blocks.Clear();
        blocks.AddRange(FindObjectsOfType<MassBlock>());
    }
}
