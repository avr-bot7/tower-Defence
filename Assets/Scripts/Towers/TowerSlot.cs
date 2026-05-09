using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    public bool       occupied     = false;
    [HideInInspector] public GameObject placedTower;

    Renderer _rend;
    Color    _defaultColor;

    public Color validColor    = new Color(0.2f, 1f, 0.2f, 0.5f);
    public Color occupiedColor = new Color(1f, 0.3f, 0.3f, 0.4f);

    void Start()
    {
        _rend = GetComponent<Renderer>();
        if (_rend) _defaultColor = _rend.material.color;

        // CRITICAL FIX: reset collider to match tile size exactly
        var col = GetComponent<BoxCollider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = false;
        col.center    = new Vector3(0, 0, 0);
        col.size      = new Vector3(1f, 0.15f, 1f); // matches 1x1 tile
    }

    public void SetHighlight(bool on)
    {
        if (_rend == null) return;
        _rend.material.color = on ? validColor : _defaultColor;
    }
    
    // ... rest of your MouseDown logic ...
}