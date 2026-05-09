using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndraLightningTower : MonoBehaviour
{
    [Header("Chain Settings")]
    public int chainCount = 4;
    public float chainRadius = 3.5f;
    public float damageFalloff = 0.7f;

    [Header("VFX")]
    public GameObject lightningVFXPrefab;

    TowerBase _base;
    float _cd;

    void Start() => _base = GetComponent<TowerBase>();

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd > 0f) return;

        Enemy target = _base.GetTargetPublic();
        if (target == null) return;

        StartCoroutine(ChainLightning(target));
        _cd = 1f / _base.currentFireRate;
    }

    IEnumerator ChainLightning(Enemy first)
    {
        var hit = new HashSet<Enemy>();
        Enemy current = first;
        float damage = _base.currentDamage;
        Vector3 fromPosition = _base.shootPoint.position;

        for (int i = 0; i < chainCount; i++)
        {
            if (current == null) break;

            hit.Add(current);
            current.TakeDamage(damage);

            SpawnLightningVFX(fromPosition, current.transform.position);
            fromPosition = current.transform.position;

            Enemy next = null;
            float best = float.MaxValue;
            var nearby = Physics.OverlapSphere(current.transform.position, chainRadius);
            foreach (var colliderHit in nearby)
            {
                var enemy = colliderHit.GetComponent<Enemy>();
                if (enemy == null || hit.Contains(enemy)) continue;

                float dist = Vector3.Distance(current.transform.position, enemy.transform.position);
                if (dist < best)
                {
                    best = dist;
                    next = enemy;
                }
            }

            damage *= damageFalloff;
            current = next;

            yield return new WaitForSeconds(0.05f);
        }
    }

    void SpawnLightningVFX(Vector3 from, Vector3 to)
    {
        if (lightningVFXPrefab == null) return;

        var go = Instantiate(lightningVFXPrefab, from, Quaternion.identity);
        var lineRenderer = go.GetComponent<LineRenderer>();
        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
        }

        Destroy(go, 0.15f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chainRadius);
    }
}
