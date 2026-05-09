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
        t.transform.position = GetTopCenterPlacementPosition(_pendingSlot, t);
        t.GetComponent<TowerBase>().Init(data);

        _pendingSlot.occupied    = true;
        _pendingSlot.placedTower = t;

        Close();
    }

    // Calculates the top-surface centre of the slot so towers sit on top of
    // the tile rather than intersecting or floating above it.
    Vector3 GetTopCenterPlacementPosition(TowerSlot slot, GameObject tower)
    {
        Vector3 pos = slot.transform.position;

        var slotCollider = slot.GetComponent<Collider>();
        if (slotCollider != null)
        {
            pos.x = slotCollider.bounds.center.x;
            pos.z = slotCollider.bounds.center.z;
        }

        float slotTopY = slotCollider != null ? slotCollider.bounds.max.y : slot.transform.position.y;

        // Use only root collider to avoid child VFX bounds pushing the tower into the sky.
        var towerCollider = tower.GetComponent<Collider>();
        float halfHeight = towerCollider != null ? towerCollider.bounds.extents.y : 0f;

        pos.y = slotTopY + halfHeight + 0.01f;
        return pos;
    }
}