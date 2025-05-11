using UnityEngine;

public class TorqueVisualizer : MonoBehaviour
{
    [Header("Seesaw Referansı")]
    public Transform seesawTransform;

    [Header("Fizik Ayarları")]
    public float gravity = 9.81f;
    [Tooltip("Tork farkı bu değerin altındaysa denge kabul edilir.")]
    public float balanceTolerance = 10f;

    void Update()
    {
        if (seesawTransform == null) return;

        float leftTorque = 0f;
        float rightTorque = 0f;
        var blocks = Object.FindObjectsByType<MassBlock>(FindObjectsSortMode.None);

        foreach (var block in blocks)
        {
            if (block == null || !block.HasBeenPlaced()) continue;

            float dx = seesawTransform.InverseTransformPoint(block.transform.position).x;
            float force = block.blockMass * gravity;
            float torque = Mathf.Abs(dx) * force;

            if (dx < 0f) leftTorque += torque;
            else rightTorque += torque;
        }

        float diff = Mathf.Abs(leftTorque - rightTorque);

        // Artık ne renk değiştiriyor, ne de angularVelocity sıfırlıyor.
        // İsterseniz burada başka bir işlem koyabilirsiniz:
        // if (diff <= balanceTolerance) { … } else { … }
    }
}
