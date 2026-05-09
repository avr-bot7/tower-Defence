// ============================================================
using UnityEngine;
using TMPro;

public class TowerInfoUI : MonoBehaviour
{
    public static TowerInfoUI I;

    public GameObject         panel;
    public TextMeshProUGUI    nameText;
    public TextMeshProUGUI    statsText;
    public TextMeshProUGUI    upgradeCostText;
    public UnityEngine.UI.Button upgradeBtn;

    TowerBase _selected;

    void Awake() { I = this; panel.SetActive(false); }

    public void Open(TowerBase t)
    {
        _selected = t;
        panel.SetActive(true);
        Refresh();
    }

    public void Close() { panel.SetActive(false); _selected = null; }

    public void OnUpgrade()
    {
        if (_selected == null) return;
        _selected.Upgrade();
        Refresh();
    }

    // Called by targeting mode dropdown (pass int 0-3)
    public void SetTargetMode(int mode)
    {
        if (_selected) _selected.targetMode = (TargetMode)mode;
    }

    void Refresh()
    {
        if (_selected == null) return;
        nameText.text  = _selected.data.towerName + $" (Lv {_selected.upgradeLevel + 1})";
        statsText.text = $"DMG: {_selected.currentDamage:F1}  " +
                         $"Rate: {_selected.currentFireRate:F1}/s  " +
                         $"Range: {_selected.currentRange:F1}";

        bool canUp = _selected.CanUpgrade();
        upgradeCostText.text = canUp ? $"Upgrade: {_selected.UpgradeCost()} Gold" : "MAX";
        upgradeBtn.interactable = canUp;
    }
}