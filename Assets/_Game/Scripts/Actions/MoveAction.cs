using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private Animator unitAnimator; 
    
    private List<Vector3> positionList;
    private int currentPositionIndex;

    private void Update()
    {
        if (!isActionInProgress) return;

        // Logic to move along the points in the list
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = .1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            
            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                isActionInProgress = false;
                onActionComplete();
            }
        }
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositions = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition);
        
        // This shouldn't happen if IsValidActionGridPosition checks are correct, but good for safety
        if (pathGridPositions == null) 
        {
            return;
        }

        this.currentPositionIndex = 0;
        this.positionList = new List<Vector3>();

        // Convert GridPositions to World Positions for the movement logic
        foreach (GridPosition pathGridPosition in pathGridPositions)
        {
            this.positionList.Add(GridSystem.Instance.GetWorldPosition(pathGridPosition));
        }

        this.onActionComplete = onActionComplete;
        this.isActionInProgress = true;
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        // 1. Is it a valid spot on the map?
        if (!GridSystem.Instance.IsValidGridPosition(gridPosition)) return false;

        // 2. Is it occupied?
        if (GridSystem.Instance.GetGridObject(gridPosition).GetUnit() != null) return false;

        // 3. Is it inside our max range? (Linear check is cheap, Pathfinding is expensive)
        // We do a quick Manhattan check first to filter out obviously far tiles.
        if (gridPosition.Distance(unit.GetGridPosition()) > maxMoveDistance) return false;

        // 4. Is it actually reachable via Pathfinding? (The expensive check)
        List<GridPosition> path = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition);
        if (path == null) return false;

        // 5. Is the path length within our movement budget?
        // Note: path count includes start or end node depending on implementation. 
        // Our CalculatePath returns the full trail.
        int pathDistance = path.Count - 1; // -1 because we don't count the tile we are standing on
        return pathDistance <= maxMoveDistance;
    }
}