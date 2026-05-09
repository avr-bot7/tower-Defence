using UnityEngine;

public class TowerBarUI : MonoBehaviour
{
    public static TowerBarUI I;

    public DragTowerIcon[] icons;

    void Awake() => I = this;

    // Call this every time gold changes
    public void RefreshAffordability()
    {
        if (icons == null) return;

        foreach (var icon in icons)
        {
            if (icon == null || icon.towerData == null || GameManager.I == null) continue;

            bool canAfford = GameManager.I.gold >= icon.towerData.cost;
            var cg = icon.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = canAfford ? 1f : 0.45f;
        }
    }
}
