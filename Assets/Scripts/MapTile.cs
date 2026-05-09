using UnityEngine;

public enum TileType
{
    Path,
    Buildable,
    Scenery,
    Water
}

public class MapTile : MonoBehaviour
{
    public TileType type;
    public bool hasTower;

    [Header("Highlight")]
    public Renderer tileRenderer;
    public Color defaultColor = Color.white;
    public Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    public Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.5f);

    MaterialPropertyBlock _materialPropertyBlock;

    public enum HighlightState
    {
        None,
        Valid,
        Invalid
    }

    void Start()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        SetHighlight(HighlightState.None);
    }

    public void SetHighlight(HighlightState state)
    {
        if (tileRenderer == null) return;

        tileRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor("_BaseColor", state switch
        {
            HighlightState.Valid => validColor,
            HighlightState.Invalid => invalidColor,
            _ => defaultColor
        });
        tileRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
