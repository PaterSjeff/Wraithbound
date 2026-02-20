using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem gridSystem;
    private Unit unit;
    private StaticObject staticObject;
    private TileType_SO tileType;
    private bool isWalkable = true;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition, TileType_SO tileType = null)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        this.tileType = tileType;
    }

    public void SetUnit(Unit unit) => this.unit = unit;
    public Unit GetUnit() => unit;

    public void SetStaticObject(StaticObject staticObject) => this.staticObject = staticObject;
    public StaticObject GetStaticObject() => staticObject;
    
    public void SetTileType(TileType_SO type) => tileType = type;
    public TileType_SO GetTileType() => tileType;

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    /// <summary>
    /// A tile is walkable if: base walkable AND (no static object OR static doesn't block).
    /// </summary>
    public bool IsWalkable()
    {
        if (!isWalkable) return false;
        
        // Check tile type blocking
        if (tileType != null && tileType.blocksMovement) return false;
        
        // Check static object blocking
        if (staticObject != null && staticObject.Data != null && staticObject.Data.blocksMovement)
            return false;
        
        return true;
    }
    
    public float GetMovementCost()
    {
        if (tileType != null)
            return tileType.movementCostMultiplier;
        return 1f;
    }

    public bool IsOccupied()
    {
        return unit != null;
    }

    public GridPosition GetGridPosition() => gridPosition;

    public override string ToString()
    {
        return gridPosition.ToString() + "\n" + (unit != null ? unit.ToString() : (staticObject != null ? staticObject.ToString() : ""));
    }
}
