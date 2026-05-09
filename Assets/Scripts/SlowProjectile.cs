using System.Collections;
using UnityEngine;

public class SlowProjectile : MonoBehaviour
{
    Enemy _target;
    float _damage;
    float _slowAmount;
    float _slowDuration;
    float _speed = 15f;

    public void Init(Enemy target, float damage, float slowAmount, float slowDuration)
    {
        _target = target;
        _damage = damage;
        _slowAmount = slowAmount;
        _slowDuration = slowDuration;
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);
        transform.LookAt(_target.transform.position);

        if (Vector3.Distance(transform.position, _target.transform.position) < 0.3f)
        {
            _target.TakeDamage(_damage);
            StartCoroutine(SlowAndDestroy(_target));
            enabled = false;
        }
    }

    IEnumerator SlowAndDestroy(Enemy enemy)
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float original = enemy.speed;
        enemy.speed *= _slowAmount;

        yield return new WaitForSeconds(_slowDuration);

        if (enemy != null)
            enemy.speed = original;

        Destroy(gameObject);
    }
}
