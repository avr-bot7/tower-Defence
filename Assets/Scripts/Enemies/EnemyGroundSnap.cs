using UnityEngine;

public class EnemyGroundSnap : MonoBehaviour
{
    [Tooltip("Layer(s) to consider as ground. Must include the layer used by your Blender map mesh.")]
    public LayerMask groundLayer = ~0;   // default: all layers
    public float snapOffset   = 0f;     // fine-tune if enemy floats slightly
    public float raycastOriginHeight = 5f; // how far above the enemy to start the downward raycast

    void LateUpdate()
    {
        // Cast a ray straight down from above the enemy to find the actual ground surface.
        // This handles custom Blender maps at any Y position, not just Y = 0.
        Vector3 origin = transform.position;
        origin.y += raycastOriginHeight;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastOriginHeight * 2f, groundLayer))
        {
            Vector3 p = transform.position;
            p.y = hit.point.y + snapOffset;
            transform.position = p;
        }
    }
}