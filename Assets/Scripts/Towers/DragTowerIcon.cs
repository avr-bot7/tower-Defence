using UnityEngine;
using UnityEngine.EventSystems;

public class DragTowerIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Info")]
    public TowerData towerData;

    [Header("UI")]
    public TMPro.TextMeshProUGUI costText;
    public TMPro.TextMeshProUGUI nameText;

    [Header("Layer")]
    public LayerMask tileLayer;

    GameObject _ghost;
    Camera _cam;
    TowerSlot _hoveredSlot;

    void Start()
    {
        _cam = Camera.main;
        if (costText) costText.text = towerData.cost + "g";
        if (nameText) nameText.text = towerData.towerName;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (GameManager.I.gold < towerData.cost) return;

        _ghost = Instantiate(towerData.prefab);
        DisableGhostScripts(_ghost);
        SetGhostAlpha(_ghost, 0.5f);
        HighlightSlots(true);
    }

    public void OnDrag(PointerEventData e)
    {
        if (_ghost == null) return;

        Ray ray = _cam.ScreenPointToRay(e.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 300f, tileLayer))
        {
            _ghost.transform.position = hit.point + Vector3.up * 0.05f;

            var slot = hit.collider.GetComponent<TowerSlot>();
            if (slot != _hoveredSlot)
            {
                if (_hoveredSlot) _hoveredSlot.SetHighlight(false);
                _hoveredSlot = slot;
                if (_hoveredSlot && !_hoveredSlot.occupied)
                    _hoveredSlot.SetHighlight(true);
            }
        }
        else
        {
            if (_hoveredSlot) { _hoveredSlot.SetHighlight(false); _hoveredSlot = null; }
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        HighlightSlots(false);
        if (_hoveredSlot) _hoveredSlot.SetHighlight(false);

        if (_ghost != null)
        {
            if (_hoveredSlot != null && !_hoveredSlot.occupied
                && GameManager.I.gold >= towerData.cost)
            {
                var tower = Instantiate(towerData.prefab,
                    _hoveredSlot.transform.position,
                    Quaternion.identity);
                tower.transform.position = GetTopCenterPlacementPosition(_hoveredSlot, tower);
                tower.GetComponent<TowerBase>().Init(towerData);
                _hoveredSlot.occupied = true;
                _hoveredSlot.placedTower = tower;
                GameManager.I.SpendGold(towerData.cost);
            }
            Destroy(_ghost);
        }

        _ghost = null;
        _hoveredSlot = null;
    }

    void SetGhostAlpha(GameObject go, float a)
    {
        foreach (var r in go.GetComponentsInChildren<Renderer>())
        {
            var mat = new Material(r.sharedMaterial);
            var c = mat.color; c.a = a; mat.color = c;
            mat.SetFloat("_Surface", 1);
            mat.renderQueue = 3000;
            r.material = mat;
        }
    }

    void DisableGhostScripts(GameObject go)
    {
        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;
        foreach (var col in go.GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    void HighlightSlots(bool on)
    {
        foreach (var s in FindObjectsOfType<TowerSlot>())
            if (!s.occupied) s.SetHighlight(on);
    }

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
        float halfHeight = 0f;

        // Use only root collider to avoid child VFX/render bounds pushing towers into the sky.
        var towerCollider = tower.GetComponent<Collider>();
        if (towerCollider != null)
            halfHeight = towerCollider.bounds.extents.y;

        pos.y = slotTopY + halfHeight + 0.01f;
        return pos;
    }
}
