using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerShopUI : MonoBehaviour
{
    public static TowerShopUI I;

    public GameObject panel;

    [Header("Tower Data — drag ScriptableObjects here")]
    public TowerData agniData;
    public TowerData vayuData;
    public TowerData indraData;
    public TowerData vishnuData;

    TowerSlot _pendingSlot;

    void Awake() { I = this; panel.SetActive(false); }

    public void Open(TowerSlot slot)
    {
        _pendingSlot = slot;
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
        _pendingSlot = null;
    }

    // Wire each button to these:
    public void BuyAgni()   => BuyTower(agniData);
    public void BuyVayu()   => BuyTower(vayuData);
    public void BuyIndra()  => BuyTower(indraData);
    public void BuyVishnu() => BuyTower(vishnuData);

    void BuyTower(TowerData data)
    {
        if (_pendingSlot == null) return;

        if (!GameManager.I.SpendGold(data.cost))
        {
            Debug.Log("Not enough gold!");
            Close();
            return;
        }

        var t = Instantiate(data.prefab, _pendingSlot.transform.position, Quaternion.identity);
        t.GetComponent<TowerBase>().Init(data);

        _pendingSlot.occupied    = true;
        _pendingSlot.placedTower = t;

        Close();
    }
}