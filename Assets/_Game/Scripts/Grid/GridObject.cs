using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem gridSystem;
    private Unit unit;
    private StaticObject staticObject;
    private bool isWalkable = true;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    public void SetUnit(Unit unit) => this.unit = unit;
    public Unit GetUnit() => unit;

    public void SetStaticObject(StaticObject staticObject) => this.staticObject = staticObject;
    public StaticObject GetStaticObject() => staticObject;

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
        if (staticObject == null) return true;
        if (staticObject.Data == null) return false;
        return !staticObject.Data.blocksMovement;
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
