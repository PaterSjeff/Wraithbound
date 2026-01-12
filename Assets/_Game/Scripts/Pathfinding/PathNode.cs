using UnityEngine;

public class PathNode 
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public GridPosition GetGridPosition() => gridPosition;

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
        CalculateFCost();
    }

    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
        CalculateFCost();
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        this.cameFromPathNode = pathNode;
    }

    public PathNode GetCameFromPathNode() => cameFromPathNode;

    public override string ToString()
    {
        return gridPosition.ToString();
    }
}