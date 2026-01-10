using UnityEngine;

public class GridObject 
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private Unit unitOnTile;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }

    // We will use this later to determine if a move is valid
    public void SetUnit(Unit unit)
    {
        this.unitOnTile = unit;
    }

    public Unit GetUnit()
    {
        return unitOnTile;
    }

    public override string ToString()
    {
        return gridPosition.ToString() + "\n" + (unitOnTile != null ? unitOnTile.name : "Empty");
    }
}