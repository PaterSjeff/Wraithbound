using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Tactical Combat/Level Data")]
public class LevelData_SO : ScriptableObject
{
    [Header("Level Info")]
    public string levelName = "New Level";
    public int levelNumber = 1;
    
    [Header("Grid Size")]
    public int width = 10;
    public int height = 10;
    
    [Header("Tile Configuration")]
    public TileType_SO defaultTileType;
    public List<TileConfiguration> tileConfigurations = new List<TileConfiguration>();
    
    [System.Serializable]
    public class TileConfiguration
    {
        public string configName;
        public TileType_SO tileType;
        public List<SerializableGridPosition> positions = new List<SerializableGridPosition>();
    }
    
    [System.Serializable]
    public struct SerializableGridPosition
    {
        public int x;
        public int z;
        
        public SerializableGridPosition(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
        
        public GridPosition ToGridPosition()
        {
            return new GridPosition(x, z);
        }
        
        public static SerializableGridPosition FromGridPosition(GridPosition pos)
        {
            return new SerializableGridPosition(pos.x, pos.z);
        }
    }
    
    public void AddTileConfiguration(TileType_SO tileType, List<GridPosition> positions, string name = null)
    {
        var config = new TileConfiguration
        {
            configName = name ?? $"{tileType.tileName} ({positions.Count} tiles)",
            tileType = tileType,
            positions = new List<SerializableGridPosition>()
        };
        
        foreach (var pos in positions)
        {
            config.positions.Add(SerializableGridPosition.FromGridPosition(pos));
        }
        
        tileConfigurations.Add(config);
    }
    
    public void ClearTileConfigurations()
    {
        tileConfigurations.Clear();
    }
}
