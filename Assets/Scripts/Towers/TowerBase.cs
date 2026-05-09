// ============================================================
using System.Collections.Generic;
using UnityEngine;

public enum TargetMode { First, Last, Strongest, Closest }

public class TowerBase : MonoBehaviour
{
    // Populated at runtime from TowerData
    [HideInInspector] public TowerData data;
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentFireRate;
    [HideInInspector] public float currentRange;
    [HideInInspector] public int   upgradeLevel = 0;

    public TargetMode targetMode = TargetMode.First;
    public bool rotateToTarget = false;

    [Header("References")]
    public Transform  shootPoint;
    public GameObject rangeIndicator;   // a flat circle mesh child

    float _fireCooldown = 0f;

    // ── Init ────────────────────────────────────────────────
    public void Init(TowerData d)
    {
        data             = d;
        currentDamage    = d.damage;
        currentFireRate  = d.fireRate;
        currentRange     = d.range;
        RefreshRangeVisual();
    }

    // ── Update ──────────────────────────────────────────────
    void Update()
    {
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown > 0) return;

        Enemy target = GetTarget();
        if (target == null) return;

        Shoot(target);
        _fireCooldown = 1f / currentFireRate;
    }

    // ── Targeting ───────────────────────────────────────────
    Enemy GetTarget()
    {
        var all = FindEnemiesInRange();
        if (all.Count == 0) return null;

        return targetMode switch
        {
            TargetMode.First    => FurthestAlongPath(all),
            TargetMode.Last     => ClosestToStart(all),
            TargetMode.Strongest=> HighestHP(all),
            TargetMode.Closest  => NearestToTower(all),
            _                   => all[0]
        };
    }

    public Enemy GetTargetPublic() => GetTarget();

    List<Enemy> FindEnemiesInRange()
    {
        var result = new List<Enemy>();
        var hits   = Physics.OverlapSphere(transform.position, currentRange);
        foreach (var h in hits)
        {
            var e = h.GetComponent<Enemy>();
            if (e == null) continue;

            var vimana = e.GetComponent<VimanaEnemy>();
            if (vimana != null && !CompareTag("AntiAir")) continue;

            result.Add(e);
        }
        return result;
    }

    Enemy FurthestAlongPath(List<Enemy> list)
    {
        Enemy best = null; int bestIdx = -1;
        foreach (var e in list)
        {
            int idx = e.GetComponent<EnemyMovement>().WaypointIndex;
            if (idx > bestIdx) { bestIdx = idx; best = e; }
        }
        return best;
    }

    Enemy ClosestToStart(List<Enemy> list)
    {
        Enemy best = null; int bestIdx = int.MaxValue;
        foreach (var e in list)
        {
            int idx = e.GetComponent<EnemyMovement>().WaypointIndex;
            if (idx < bestIdx) { bestIdx = idx; best = e; }
        }
        return best;
    }

    Enemy HighestHP(List<Enemy> list)
    {
        Enemy best = null; float bestHP = -1;
        foreach (var e in list)
        {
            float hp = e.GetComponent<Enemy>().maxHP;
            if (hp > bestHP) { bestHP = hp; best = e; }
        }
        return best;
    }

    Enemy NearestToTower(List<Enemy> list)
    {
        Enemy best = null; float bestDist = float.MaxValue;
        foreach (var e in list)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < bestDist) { bestDist = d; best = e; }
        }
        return best;
    }

    // ── Shooting ────────────────────────────────────────────
    void Shoot(Enemy target)
    {
        Debug.Log("Shooting enemy: " + target.name);

        // Rotate toward target
        if (rotateToTarget)
        {
            Vector3 dir = (target.transform.position - transform.position);
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        // Trigger muzzle flash if present
        GetComponentInChildren<MuzzleFlash>()?.Flash();

        // If no projectile prefab - instant damage
        if (data == null || data.projectilePrefab == null)
        {
            target.TakeDamage(currentDamage);
            return;
        }

        // Spawn projectile
        if (shootPoint == null) return;
        var proj = Instantiate(data.projectilePrefab,
            shootPoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>()?.Init(target, currentDamage);
    }

    // ── Upgrading ───────────────────────────────────────────
    public bool CanUpgrade() => upgradeLevel < data.upgrades.Length;

    public int UpgradeCost() =>
        CanUpgrade() ? data.upgrades[upgradeLevel].cost : 0;

    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        var tier      = data.upgrades[upgradeLevel];
        if (!GameManager.I.SpendGold(tier.cost)) return;

        currentDamage   += tier.damageBonus;
        currentFireRate += tier.fireRateBonus;
        currentRange    += tier.rangeBonus;
        upgradeLevel++;
        RefreshRangeVisual();
    }

    // ── Helpers ─────────────────────────────────────────────
    void RefreshRangeVisual()
    {
        if (rangeIndicator)
            rangeIndicator.transform.localScale = new Vector3(currentRange * 0.1f, 0.1f, currentRange * 0.1f);
    }

    void OnMouseDown() => TowerInfoUI.I.Open(this);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }
}
