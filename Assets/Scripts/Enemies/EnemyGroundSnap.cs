using UnityEngine;

public class EnemyGroundSnap : MonoBehaviour
{
    public float groundY      = 0f;    // set this to your tile surface Y
    public float snapOffset   = 0f;    // fine-tune if enemy floats slightly

    void LateUpdate()
    {
        Vector3 p = transform.position;
        p.y = groundY + snapOffset;
        transform.position = p;
    }
}