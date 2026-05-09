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
        transform.LookAt(target.position);   // face movement direction

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            _waypointIdx++;
            if (_waypointIdx >= WaypointPath.I.Length)
                _enemy.ReachEnd();
        }
    }
}