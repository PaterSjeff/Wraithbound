using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    // Standard A* Costs
    private const int MOVE_STRAIGHT_COST = 10;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    
    private PathNode[,] pathNodes;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Ensure this matches GridSystem size if hardcoded, 
        // ideally getting these values from GridSystem is cleaner 
        // but for this prototype stage, this works.
        pathNodes = new PathNode[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                pathNodes[x, z] = new PathNode(new GridPosition(x, z));
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        // Validation
        if (!GridSystem.Instance.IsValidGridPosition(endGridPosition)) return null;
        if (!GridSystem.Instance.IsValidGridPosition(startGridPosition)) return null;

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetNode(startGridPosition);
        PathNode endNode = GetNode(endGridPosition);
        
        openList.Add(startNode);

        // Reset nodes for calculation
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                PathNode pathNode = pathNodes[x, z];
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.SetCameFromPathNode(null);
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                
                // Collision Check: Ask GridSystem if this tile is occupied
                // We allow the "End Node" to be occupied (for melee attacks later)
                // But we cannot walk THROUGH an occupied node.
                bool isOccupied = GridSystem.Instance.GetGridObject(neighbourNode.GetGridPosition()).GetUnit() != null;
                
                if (isOccupied && neighbourNode != endNode)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());
                
                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    private int CalculateDistance(GridPosition a, GridPosition b)
    {
        GridPosition gridDistance = new GridPosition(Mathf.Abs(a.x - b.x), Mathf.Abs(a.z - b.z));
        // Manhattan Distance for 4-way movement
        return MOVE_STRAIGHT_COST * (gridDistance.x + gridDistance.z);
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostNode.GetFCost())
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private PathNode GetNode(GridPosition gridPosition)
    {
        return pathNodes[gridPosition.x, gridPosition.z];
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0) neighbourList.Add(GetNode(new GridPosition(gridPosition.x - 1, gridPosition.z))); // Left
        if (gridPosition.x + 1 < width) neighbourList.Add(GetNode(new GridPosition(gridPosition.x + 1, gridPosition.z))); // Right
        if (gridPosition.z - 1 >= 0) neighbourList.Add(GetNode(new GridPosition(gridPosition.x, gridPosition.z - 1))); // Down
        if (gridPosition.z + 1 < height) neighbourList.Add(GetNode(new GridPosition(gridPosition.x, gridPosition.z + 1))); // Up

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            path.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        path.Reverse();
        
        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in path)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }
        return gridPositionList;
    }
}