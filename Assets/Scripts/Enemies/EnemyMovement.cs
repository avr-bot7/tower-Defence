// ============================================================
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    int   _waypointIdx = 0;
    Enemy _enemy;

    public int WaypointIndex => _waypointIdx;

    void Start() => _enemy = GetComponent<Enemy>();

    void Update()
    {
        if (WaypointPath.I == null) return;

        Transform target = WaypointPath.I.Get(_waypointIdx);
        Vector3   dir    = (target.position - transform.position).normalized;

        transform.position += dir * _enemy.speed * Time.deltaTime;

        // Rotate only on the Y axis so the enemy doesn't pitch into slopes.
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > Mathf.Epsilon)
            transform.rotation = Quaternion.LookRotation(lookDir);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            _waypointIdx++;
            if (_waypointIdx >= WaypointPath.I.Length)
                _enemy.ReachEnd();
        }
    }
}