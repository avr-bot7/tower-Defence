using System.Collections;
using UnityEngine;

public class VayuTower : MonoBehaviour
{
    [Header("Arrow Settings")]
    public float slowAmount = 0.5f;
    public float slowDuration = 1.5f;

    [Header("VFX")]
    public GameObject arrowPrefab;

    TowerBase _base;
    float _cd;

    void Start() => _base = GetComponent<TowerBase>();

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd > 0f) return;

        Enemy target = _base.GetTargetPublic();
        if (target == null) return;

        FireArrow(target);
        _cd = 1f / _base.currentFireRate;
    }

    void FireArrow(Enemy target)
    {
        if (arrowPrefab)
        {
            var arrow = Instantiate(arrowPrefab, _base.shootPoint.position, Quaternion.identity);
            var projectile = arrow.AddComponent<SlowProjectile>();
            projectile.Init(target, _base.currentDamage, slowAmount, slowDuration);
        }
        else
        {
            target.TakeDamage(_base.currentDamage);
            StartCoroutine(SlowEnemy(target));
        }
    }

    IEnumerator SlowEnemy(Enemy enemy)
    {
        if (enemy == null) yield break;
        if (enemy.GetComponent<MahishasuraBoss>()?.IsSlowImmune() == true) yield break;

        float originalSpeed = enemy.speed;
        enemy.speed *= slowAmount;

        yield return new WaitForSeconds(slowDuration);

        if (enemy != null)
            enemy.speed = originalSpeed;
    }
}
