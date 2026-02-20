using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer highlightRenderer;
    [SerializeField] private MeshRenderer hoverRenderer;
    [SerializeField] private MeshRenderer tileRenderer;

    private GridObject gridObject;
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        UpdateTileVisual();
    }

    public GridObject GetGridObject() => gridObject;

    public void UpdateTileVisual()
    {
        if (tileRenderer == null)
        {
            Debug.LogWarning($"[GridDebugObject] TileRenderer not assigned on {name}!");
            return;
        }

        if (gridObject == null)
        {
            Debug.LogWarning($"[GridDebugObject] GridObject is null on {name}!");
            return;
        }

        TileType_SO tileType = gridObject.GetTileType();
        if (tileType != null)
        {
            tileRenderer.enabled = true;
            if (tileRenderer.material != null)
            {
                tileRenderer.material.color = tileType.tileColor;
                Debug.Log($"[GridDebugObject] Set tile at {gridObject.GetGridPosition()} to color {tileType.tileColor} ({tileType.tileName})");
            }
            else
            {
                Debug.LogWarning($"[GridDebugObject] TileRenderer material is null on {name}!");
            }
        }
        else
        {
            tileRenderer.enabled = false;
            Debug.Log($"[GridDebugObject] No tile type set at {gridObject.GetGridPosition()}, disabling renderer");
        }
    }

    public void SetHighlight(bool enabled, Color color)
    {
        if (highlightRenderer != null)
        {
            highlightRenderer.enabled = enabled;
            if (enabled && highlightRenderer.material != null)
            {
                highlightRenderer.material.color = color;
            }
        }
    }

    public void ClearHighlight()
    {
        if (highlightRenderer != null)
            highlightRenderer.enabled = false;
    }

    public void SetHoverPreview(bool enabled, Color color)
    {
        if (hoverRenderer != null)
        {
            hoverRenderer.enabled = enabled;
            if (enabled && hoverRenderer.material != null)
            {
                hoverRenderer.material.color = color;
            }
        }
    }

    public void ClearHoverPreview()
    {
        if (hoverRenderer != null)
            hoverRenderer.enabled = false;
    }

    private void Update()
    {
        if (textMeshPro != null && gridObject != null)
        {
            string text = gridObject.ToString();
            
            // Add tile type info
            TileType_SO tileType = gridObject.GetTileType();
            if (tileType != null)
            {
                text += $"\n[{tileType.tileName}]";
            }
            
            textMeshPro.text = text;
        }
    }
}
