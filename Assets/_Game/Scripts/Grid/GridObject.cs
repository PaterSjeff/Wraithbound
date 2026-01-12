// Path: Scripts/Grid/GridObject.cs
using System.Collections.Generic;
using UnityEngine;

public class GridObject 
{
    private GridPosition gridPosition;
    private GridSystem gridSystem;
    private Unit unit;
    
    // NEW: Track if this specific tile is walkable
    private bool isWalkable = true;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    public void SetUnit(Unit unit) => this.unit = unit;
    public Unit GetUnit() => unit;

    // NEW: Getters and Setters for walkability
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public override string ToString()
    {
        return gridPosition.ToString() + "\n" + unit;
    }
}