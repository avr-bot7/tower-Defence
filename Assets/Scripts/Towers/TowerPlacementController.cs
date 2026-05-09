using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController I;

    [Header("Preview")]
    public LayerMask tileLayer;
    public LayerMask towerLayer;

    GameObject _previewObject;
    TowerData _pendingData;
    bool _placing;

    void Awake()
    {
        I = this;
    }

    public void BeginPlacement(TowerData data, GameObject previewPrefab)
    {
        if (data == null || previewPrefab == null) return;

        _pendingData = data;
        _placing = true;
        _previewObject = Instantiate(previewPrefab);

        foreach (var rendererComp in _previewObject.GetComponentsInChildren<Renderer>())
        {
            var mat = rendererComp.material;
            var color = mat.color;
            color.a = 0.5f;
            mat.color = color;
        }

        HighlightBuildableTiles(true);
    }

    void Update()
    {
        if (!_placing || _previewObject == null) return;
        if (Camera.main == null) return;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            if (Input.GetMouseButtonDown(0))
                CancelPlacement();
            return;
        }

        var tile = hit.collider.GetComponent<MapTile>();
        if (tile == null)
        {
            if (Input.GetMouseButtonDown(0))
                CancelPlacement();
            return;
        }

        bool canPlace = tile.type == TileType.Buildable && !tile.hasTower;
        _previewObject.transform.position = tile.transform.position;
        tile.SetHighlight(canPlace ? MapTile.HighlightState.Valid : MapTile.HighlightState.Invalid);

        if (Input.GetMouseButtonDown(0) && canPlace)
            ConfirmPlacement(tile);

        if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    void ConfirmPlacement(MapTile tile)
    {
        if (_pendingData == null || _pendingData.prefab == null) return;
        if (!GameManager.I.SpendGold(_pendingData.cost)) return;

        var tower = Instantiate(_pendingData.prefab, tile.transform.position, Quaternion.identity);
        tower.transform.position = GetTopCenterPlacementPosition(tile, tower);
        var towerBase = tower.GetComponent<TowerBase>();
        if (towerBase != null)
            towerBase.Init(_pendingData);

        tile.hasTower = true;
        tile.SetHighlight(MapTile.HighlightState.None);

        EndPlacement();
    }

    Vector3 GetTopCenterPlacementPosition(MapTile tile, GameObject tower)
    {
        Vector3 pos = tile.transform.position;

        var tileCollider = tile.GetComponent<Collider>();
        if (tileCollider != null)
        {
            pos.x = tileCollider.bounds.center.x;
            pos.z = tileCollider.bounds.center.z;
        }

        float tileTopY = tileCollider != null ? tileCollider.bounds.max.y : tile.transform.position.y;
        float halfHeight = 0f;

        // Use only root collider to avoid child VFX/render bounds pushing towers into the sky.
        var towerCollider = tower.GetComponent<Collider>();
        if (towerCollider != null)
            halfHeight = towerCollider.bounds.extents.y;

        pos.y = tileTopY + halfHeight + 0.01f;
        return pos;
    }

    void CancelPlacement()
    {
        EndPlacement();
    }

    void EndPlacement()
    {
        _placing = false;

        if (_previewObject != null)
            Destroy(_previewObject);

        HighlightBuildableTiles(false);
    }

    void HighlightBuildableTiles(bool on)
    {
        foreach (var tile in FindObjectsOfType<MapTile>())
        {
            if (tile.type != TileType.Buildable) continue;

            tile.SetHighlight(on && !tile.hasTower
                ? MapTile.HighlightState.Valid
                : MapTile.HighlightState.None);
        }
    }
}
