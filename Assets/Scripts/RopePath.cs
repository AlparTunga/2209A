using System.Collections.Generic;
using UnityEngine;

public class RopePath : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform weight;
    public Transform[] pulleyPoints; // makara noktaları (sabit + hareketli)

    void Update()
    {
        List<Vector3> ropePositions = new List<Vector3>();

        // 1. ip yükten başlasın
        ropePositions.Add(weight.position);

        // 2. makaraların pozisyonları
        foreach (var point in pulleyPoints)
        {
            ropePositions.Add(point.position);
        }

        // 3. son durak: çekme noktası (örnek olarak sağa biraz offset)
        Vector3 anchorEnd = pulleyPoints[pulleyPoints.Length - 1].position + Vector3.right * 1f;
        ropePositions.Add(anchorEnd);

        // 4. çiz
        lineRenderer.positionCount = ropePositions.Count;
        lineRenderer.SetPositions(ropePositions.ToArray());
    }
}
