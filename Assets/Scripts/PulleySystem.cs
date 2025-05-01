using System.Collections.Generic;
using UnityEngine;

public class PulleySystem : MonoBehaviour
{
    public Transform weight;
    public Transform[] pulleyPoints;
    public LineRenderer ropeRenderer;
    public float liftSpeed = 2f;

    [Header("Sınırlama Ayarları")]
    public float maxHeight = 2.5f; // Ağırlığın çıkabileceği maksimum Y yüksekliği

    private float mechanicalAdvantage => pulleyPoints.Length + 1;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float moveAmount = liftSpeed * Time.deltaTime;

            // Yukarı sınıra geldiyse durdur
            if (weight.position.y < maxHeight)
            {
                weight.Translate(Vector3.up * (moveAmount / mechanicalAdvantage));
            }

            UpdateRopePath();
        }
        else
        {
            UpdateRopePath();
        }
    }

    void UpdateRopePath()
    {
        List<Vector3> ropePositions = new List<Vector3>();

        ropePositions.Add(weight.position);

        foreach (Transform point in pulleyPoints)
        {
            ropePositions.Add(point.position);
        }

        Vector3 endPoint = pulleyPoints[pulleyPoints.Length - 1].position + new Vector3(0.5f, -2f, 0f);

        ropePositions.Add(endPoint);

        ropeRenderer.positionCount = ropePositions.Count;
        ropeRenderer.SetPositions(ropePositions.ToArray());
    }
    
}