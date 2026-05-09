// ============================================================
using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefence/TowerData")]
public class TowerData : ScriptableObject
{
    public string     towerName;
    public int        cost;
    public float      damage;
    public float      fireRate;   // shots per second
    public float      range;
    public GameObject prefab;
    public GameObject projectilePrefab;

    // Upgrade tiers (each index = next tier)
    public TowerUpgradeTier[] upgrades;
}

[System.Serializable]
public class TowerUpgradeTier
{
    public int   cost;
    public float damageBonus;
    public float fireRateBonus;
    public float rangeBonus;
}
