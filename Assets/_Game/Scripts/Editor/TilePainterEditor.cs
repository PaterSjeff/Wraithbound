using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilePainter))]
public class TilePainterEditor : Editor
{
    private TileType_SO selectedTileType;
    private bool isPainting = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilePainter painter = (TilePainter)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Painting Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Select a tile type below, then click 'Enable Painting Mode' and click on tiles in the Scene view to paint them.", MessageType.Info);

        selectedTileType = (TileType_SO)EditorGUILayout.ObjectField("Paint Brush", selectedTileType, typeof(TileType_SO), false);

        if (GUILayout.Button(isPainting ? "Disable Painting Mode" : "Enable Painting Mode"))
        {
            isPainting = !isPainting;
            SceneView.RepaintAll();
        }

        if (isPainting && selectedTileType == null)
        {
            EditorGUILayout.HelpBox("Select a Tile Type to paint!", MessageType.Warning);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save/Load Tools", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            var paintedTiles = TilePainter.GetManualPaintedTiles();
            EditorGUILayout.HelpBox($"Currently {paintedTiles.Count} tiles manually painted this session.", MessageType.Info);
            
            if (GUILayout.Button("💾 Save Manual Painting as Paint Rules"))
            {
                if (paintedTiles.Count == 0)
                {
                    EditorUtility.DisplayDialog("Nothing to Save", "No manual painting detected. Paint some tiles first!", "OK");
                }
                else if (EditorUtility.DisplayDialog("Save Manual Painting?", 
                    $"This will add {paintedTiles.Count} manually painted tiles as Paint Rules.\n\nExisting rules will be preserved.", 
                    "Save", "Cancel"))
                {
                    SaveManualPaintingAsRules(painter, paintedTiles);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play Mode to paint and save tiles.", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Reset All Tiles (Apply Paint Rules)"))
        {
            if (Application.isPlaying && GridSystem.Instance != null)
            {
                if (EditorUtility.DisplayDialog("Reset Tiles?", 
                    "This will RESET all tiles back to the configured Default Tile Type and Paint Rules. Any unsaved manual painting will be lost!\n\nAre you sure?", 
                    "Yes, Reset", "Cancel"))
                {
                    painter.PaintTiles();
                    TilePainter.ClearManualPaintTracking();
                    EditorUtility.DisplayDialog("Tiles Reset", "Paint rules have been applied! Manual paint tracking has been cleared.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Cannot Paint", "Enter Play Mode first to paint tiles.", "OK");
            }
        }
    }

    private void OnSceneGUI()
    {
        if (!isPainting || selectedTileType == null || !Application.isPlaying || GridSystem.Instance == null)
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPos = hit.point;
                GridPosition gridPos = GridSystem.Instance.GetGridPosition(worldPos);
                
                if (GridSystem.Instance.IsValidGridPosition(gridPos))
                {
                    TilePainter.PaintTile(gridPos.x, gridPos.z, selectedTileType);
                    Debug.Log($"Painted tile at {gridPos} with {selectedTileType.tileName}");
                    e.Use();
                }
            }
        }

        // Draw brush preview
        if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPos = hit.point;
                GridPosition gridPos = GridSystem.Instance.GetGridPosition(worldPos);
                
                if (GridSystem.Instance.IsValidGridPosition(gridPos))
                {
                    Vector3 tileCenter = GridSystem.Instance.GetWorldPosition(gridPos);
                    float cellSize = GridSystem.Instance.CellSize;
                    
                    Handles.color = new Color(selectedTileType.tileColor.r, selectedTileType.tileColor.g, selectedTileType.tileColor.b, 0.5f);
                    Handles.DrawSolidRectangleWithOutline(
                        new Vector3[]
                        {
                            tileCenter + new Vector3(-cellSize * 0.5f, 0.1f, -cellSize * 0.5f),
                            tileCenter + new Vector3(cellSize * 0.5f, 0.1f, -cellSize * 0.5f),
                            tileCenter + new Vector3(cellSize * 0.5f, 0.1f, cellSize * 0.5f),
                            tileCenter + new Vector3(-cellSize * 0.5f, 0.1f, cellSize * 0.5f)
                        },
                        selectedTileType.tileColor,
                        Color.white
                    );
                    
                    SceneView.RepaintAll();
                }
            }
        }
    }
    
    private void SaveManualPaintingAsRules(TilePainter painter, Dictionary<GridPosition, TileType_SO> paintedTiles)
    {
        LevelData_SO levelData = painter.GetLevelData();
        
        if (levelData == null)
        {
            if (EditorUtility.DisplayDialog("No Level Data", 
                "No Level Data asset assigned! Would you like to create one?", 
                "Create", "Cancel"))
            {
                // Create new level data asset
                levelData = ScriptableObject.CreateInstance<LevelData_SO>();
                
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create Level Data",
                    "NewLevel",
                    "asset",
                    "Save level data as:");
                
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(levelData, path);
                    AssetDatabase.SaveAssets();
                    painter.SetLevelData(levelData);
                    EditorUtility.SetDirty(painter);
                    Debug.Log($"Created new Level Data at {path}");
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        
        // Group painted tiles by tile type
        var tilesByType = new Dictionary<TileType_SO, List<GridPosition>>();
        foreach (var kvp in paintedTiles)
        {
            if (!tilesByType.ContainsKey(kvp.Value))
                tilesByType[kvp.Value] = new List<GridPosition>();
            tilesByType[kvp.Value].Add(kvp.Key);
        }
        
        // Add configurations to level data
        int configsAdded = 0;
        foreach (var kvp in tilesByType)
        {
            TileType_SO tileType = kvp.Key;
            List<GridPosition> positions = kvp.Value;
            
            levelData.AddTileConfiguration(tileType, positions, $"Manual Paint - {tileType.tileName}");
            configsAdded++;
        }
        
        // Mark level data as dirty to save
        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
        
        // Clear tracking
        TilePainter.ClearManualPaintTracking();
        
        EditorUtility.DisplayDialog("Saved!", 
            $"Successfully saved {paintedTiles.Count} tiles as {configsAdded} configurations to Level Data!\n\nThe level data has been saved and will persist.", 
            "OK");
        
        Debug.Log($"[TilePainter] Saved {paintedTiles.Count} manually painted tiles to {levelData.name}");
    }
}
