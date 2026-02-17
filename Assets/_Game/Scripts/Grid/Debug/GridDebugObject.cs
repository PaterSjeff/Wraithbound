using UnityEngine;
using TMPro;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private MeshRenderer highlightRenderer;
    [SerializeField] private MeshRenderer hoverRenderer;

    private GridObject gridObject;
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    public GridObject GetGridObject() => gridObject;

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
        if (textMeshPro != null)
            textMeshPro.text = gridObject?.ToString() ?? "";
    }
}
