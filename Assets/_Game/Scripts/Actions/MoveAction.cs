using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    public override string GetActionName() => "Move";

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // Calculate Path (Assumes Pathfinding is set up, otherwise falls back to simple move)
        List<GridPosition> pathGridPositions = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        if (pathGridPositions != null)
        {
            foreach (GridPosition pathGridPos in pathGridPositions)
            {
                positionList.Add(GridSystem.Instance.GetWorldPosition(pathGridPos));
            }
        }
        else
        {
            // Fallback if Pathfinding fails or isn't set up yet: Direct Line
            positionList.Add(GridSystem.Instance.GetWorldPosition(gridPosition));
        }

        // Standardized Start
        BaseActionStart(onActionComplete);
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Rotate
        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }

        // Move
        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                // Standardized Complete
                BaseActionComplete();
            }
        }
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = GetValidActionGridPositionList();
        return gridPositionList.Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GridSystem.Instance.IsValidGridPosition(testGridPosition)) continue;
                if (unitGridPosition == testGridPosition) continue;
                if (GridSystem.Instance.GetGridObject(testGridPosition).GetUnit() != null) continue;

                // Pathfinding check: Is it reachable?
                if (Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList;
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }
}