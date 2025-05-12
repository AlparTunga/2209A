using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VisualRope : MonoBehaviour
{
    [Header("Anchors (hand & wagon)")]
    public Transform LeftAnchor;
    public Transform RightAnchor;

    [Header("How many segments to draw")]  
    [Range(2, 100)] public int resolution = 20;

    [Header("Maximum sag when slack (world units)")]
    public float sagAmount = 0.5f;

    [Header("When distance==maxDistance, rope is fully taut (no sag)")]
    public float maxDistance = 5f;

    [Header("Small vertical offset so rope floats above mesh")]
    public float heightOffset = 0.2f;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (LeftAnchor == null || RightAnchor == null) return;

        // compute sag factor
        float dist = Vector3.Distance(LeftAnchor.position, RightAnchor.position);
        float t = Mathf.InverseLerp(maxDistance, 0f, dist);
        float sag = sagAmount * t;

        int points = resolution + 1;
        lr.positionCount = points;

        for (int i = 0; i <= resolution; i++)
        {
            float f = (float)i / resolution;
            Vector3 basePos = Vector3.Lerp(LeftAnchor.position, RightAnchor.position, f);

            // sag curve
            float sagCurve = Mathf.Sin(Mathf.PI * f);
            Vector3 sagOffset = Vector3.down * (sag * sagCurve);

            // apply height offset
            Vector3 upOffset = Vector3.up * heightOffset;

            lr.SetPosition(i, basePos + sagOffset + upOffset);
        }
    }
}