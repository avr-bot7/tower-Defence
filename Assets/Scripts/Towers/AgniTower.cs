using System.Collections;
using UnityEngine;

public class AgniTower : MonoBehaviour
{
    [Header("AoE Settings")]
    public float splashRadius = 2.5f;
    public float burnDamage = 5f;
    public float burnDuration = 3f;
    public float burnTickRate = 0.5f;

    [Header("VFX")]
    public GameObject fireballPrefab;
    public GameObject explosionVFXPrefab;

    TowerBase _base;
    float _cd;

    void Start() => _base = GetComponent<TowerBase>();

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd > 0f) return;

        Enemy target = _base.GetTargetPublic();
        if (target == null) return;

        StartCoroutine(ShootFireball(target));
        _cd = 1f / _base.currentFireRate;
    }

    IEnumerator ShootFireball(Enemy target)
    {
        GameObject projectilePrefab = fireballPrefab;
        if (projectilePrefab == null && _base != null && _base.data != null)
            projectilePrefab = _base.data.projectilePrefab;

        if (projectilePrefab)
        {
            Vector3 spawnPosition = _base != null && _base.shootPoint != null
                ? _base.shootPoint.position
                : transform.position;

            var fireball = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            var follow = fireball.AddComponent<SimpleFollow>();
            follow.Init(target.transform, 10f);
            yield return new WaitUntil(() => follow.Arrived || target == null);
            Destroy(fireball);
        }

        if (target == null) yield break;

        if (explosionVFXPrefab)
            Instantiate(explosionVFXPrefab, target.transform.position, Quaternion.identity);

        var hits = Physics.OverlapSphere(target.transform.position, splashRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            enemy.TakeDamage(_base.currentDamage);
            StartCoroutine(ApplyBurn(enemy));
        }
    }

    IEnumerator ApplyBurn(Enemy enemy)
    {
        float elapsed = 0f;
        while (elapsed < burnDuration && enemy != null)
        {
            enemy.TakeDamage(burnDamage);
            elapsed += burnTickRate;
            yield return new WaitForSeconds(burnTickRate);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, splashRadius);
    }
}
