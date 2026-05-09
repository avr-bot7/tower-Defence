// ============================================================
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Enemy  _target;
    float  _damage;
    float  _speed = 12f;

    public void Init(Enemy target, float dmg)
    {
        _target = target;
        _damage = dmg;
    }

    void Update()
    {
        if (_target == null) { Destroy(gameObject); return; }

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.transform.position,
            _speed * Time.deltaTime);

        transform.LookAt(_target.transform.position);

        if (Vector3.Distance(transform.position,
            _target.transform.position) < 0.3f)
        {
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
