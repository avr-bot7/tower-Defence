// ============================================================
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public static WaypointPath I;

    // Drag waypoint GameObjects in order inside the Inspector
    public Transform[] points;

    void Awake() => I = this;

    public Transform Get(int idx) => points[idx % points.Length];
    public int Length => points.Length;

    // Draw path in Scene view for easy editing
    void OnDrawGizmos()
    {
        if (points == null || points.Length < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < points.Length - 1; i++)
            if (points[i] && points[i + 1])
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
    }
}
