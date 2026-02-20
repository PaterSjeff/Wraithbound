using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple utility to paint tile types on the grid
/// Attach to GameManager or create a separate GameObject
/// </summary>
public class TilePainter : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData_SO levelData;
    
    [Header("Legacy Tile Painting (Deprecated - Use Level Data)")]
    [SerializeField] private TileType_SO defaultTileType;
    [SerializeField] private TilePaintRule[] paintRules;
    
    // Track manual painting during play mode
    private static Dictionary<GridPosition, TileType_SO> manualPaintedTiles = new Dictionary<GridPosition, TileType_SO>();

    [System.Serializable]
    public class TilePaintRule
    {
        public string ruleName;
        public TileType_SO tileType;
        [Tooltip("Paint specific positions (comma separated: 0,0  1,0  2,0)")]
        public string positions;
        [Tooltip("Or paint a rectangular area")]
        public bool useArea;
        public int areaStartX;
        public int areaStartZ;
        public int areaEndX;
        public int areaEndZ;
    }

    /// <summary>
    /// Call this after GridSystem.Init() to paint tiles
    /// </summary>
    public void PaintTiles()
    {
        if (GridSystem.Instance == null)
        {
            Debug.LogError("GridSystem not initialized!");
            return;
        }

        // Use Level Data if available
        if (levelData != null)
        {
            PaintFromLevelData();
            return;
        }

        // Fall back to legacy paint rules
        PaintFromLegacyRules();
    }
    
    private void PaintFromLevelData()
    {
        int width = GridSystem.Instance.Width;
        int height = GridSystem.Instance.Height;
        
        // Apply default tile type
        if (levelData.defaultTileType != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridObject gridObj = GridSystem.Instance.GetGridObject(new GridPosition(x, z));
                    if (gridObj != null)
                    {
                        gridObj.SetTileType(levelData.defaultTileType);
                        UpdateTileVisual(x, z);
                    }
                }
            }
            Debug.Log($"[TilePainter] Painted all tiles with default: {levelData.defaultTileType.tileName}");
        }
        
        // Apply tile configurations
        foreach (var config in levelData.tileConfigurations)
        {
            if (config.tileType == null) continue;
            
            foreach (var serializablePos in config.positions)
            {
                GridPosition pos = serializablePos.ToGridPosition();
                if (GridSystem.Instance.IsValidGridPosition(pos))
                {
                    GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
                    gridObj?.SetTileType(config.tileType);
                    UpdateTileVisual(pos.x, pos.z);
                }
            }
            Debug.Log($"[TilePainter] Applied config '{config.configName}' with {config.positions.Count} tiles");
        }
    }
    
    private void PaintFromLegacyRules()
    {
        int width = GridSystem.Instance.Width;
        int height = GridSystem.Instance.Height;

        // Apply default tile type to all tiles first
        if (defaultTileType != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridObject gridObj = GridSystem.Instance.GetGridObject(new GridPosition(x, z));
                    if (gridObj != null)
                    {
                        gridObj.SetTileType(defaultTileType);
                        UpdateTileVisual(x, z);
                    }
                }
            }
            Debug.Log($"Painted all tiles with default: {defaultTileType.tileName}");
        }

        // Apply paint rules
        if (paintRules != null)
        {
            foreach (TilePaintRule rule in paintRules)
            {
                if (rule.tileType == null) continue;

                if (rule.useArea)
                {
                    // Paint rectangular area
                    for (int x = rule.areaStartX; x <= rule.areaEndX; x++)
                    {
                        for (int z = rule.areaStartZ; z <= rule.areaEndZ; z++)
                        {
                            GridPosition pos = new GridPosition(x, z);
                            if (GridSystem.Instance.IsValidGridPosition(pos))
                            {
                                GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
                                gridObj?.SetTileType(rule.tileType);
                                UpdateTileVisual(x, z);
                            }
                        }
                    }
                    Debug.Log($"Painted area ({rule.areaStartX},{rule.areaStartZ}) to ({rule.areaEndX},{rule.areaEndZ}) with {rule.tileType.tileName}");
                }
                else if (!string.IsNullOrEmpty(rule.positions))
                {
                    // Paint specific positions
                    string[] posStrings = rule.positions.Split(new char[] { ' ', '\n', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string posStr in posStrings)
                    {
                        string[] coords = posStr.Split(',');
                        if (coords.Length == 2 && int.TryParse(coords[0], out int x) && int.TryParse(coords[1], out int z))
                        {
                            GridPosition pos = new GridPosition(x, z);
                            if (GridSystem.Instance.IsValidGridPosition(pos))
                            {
                                GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
                                gridObj?.SetTileType(rule.tileType);
                                UpdateTileVisual(x, z);
                            }
                        }
                    }
                    Debug.Log($"Painted {posStrings.Length} specific positions with {rule.tileType.tileName}");
                }
            }
        }
    }

    private void UpdateTileVisual(int x, int z)
    {
        GridPosition pos = new GridPosition(x, z);
        GridDebugObject debugObj = GridSystem.Instance.GetGridDebugObject(pos);
        if (debugObj != null)
        {
            debugObj.UpdateTileVisual();
        }
        else
        {
            Debug.LogWarning($"[TilePainter] No GridDebugObject found at {pos}");
        }
    }

    /// <summary>
    /// Helper to paint a single tile (useful for scripting)
    /// </summary>
    public static void PaintTile(int x, int z, TileType_SO tileType, bool trackForSaving = true)
    {
        if (GridSystem.Instance == null) return;
        GridPosition pos = new GridPosition(x, z);
        if (GridSystem.Instance.IsValidGridPosition(pos))
        {
            GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
            gridObj?.SetTileType(tileType);
            
            // Track this for potential saving
            if (trackForSaving && Application.isPlaying)
            {
                manualPaintedTiles[pos] = tileType;
            }
            
            // Update visual directly
            GridDebugObject debugObj = GridSystem.Instance.GetGridDebugObject(pos);
            debugObj?.UpdateTileVisual();
            
            Debug.Log($"[TilePainter] Painted tile at ({x},{z}) with {tileType.tileName}");
        }
    }
    
    /// <summary>
    /// Get all manually painted tiles (for editor saving)
    /// </summary>
    public static Dictionary<GridPosition, TileType_SO> GetManualPaintedTiles()
    {
        return new Dictionary<GridPosition, TileType_SO>(manualPaintedTiles);
    }
    
    /// <summary>
    /// Clear manual painting tracking
    /// </summary>
    public static void ClearManualPaintTracking()
    {
        manualPaintedTiles.Clear();
    }
    
    /// <summary>
    /// Get current level data (for editor)
    /// </summary>
    public LevelData_SO GetLevelData()
    {
        return levelData;
    }
    
    /// <summary>
    /// Set level data (for editor)
    /// </summary>
    public void SetLevelData(LevelData_SO data)
    {
        levelData = data;
    }

    /// <summary>
    /// Helper to paint an area (useful for scripting)
    /// </summary>
    public static void PaintArea(int startX, int startZ, int endX, int endZ, TileType_SO tileType)
    {
        if (GridSystem.Instance == null) return;
        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                PaintTile(x, z, tileType);
            }
        }
    }
}
