using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessAction : BaseAction
{
    [SerializeField] private int maxPossessRange = 2; // Slightly longer than attack

    public override string GetActionName() => "Possess";

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Unit targetUnit = GridSystem.Instance.GetGridObject(gridPosition).GetUnit();
        
        // Logic handled in Unit, but Action wrapper handles the flow
        unit.PerformPossession(targetUnit);

        BaseActionStart(onActionComplete);
        BaseActionComplete(); // Ends immediately
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxPossessRange; x <= maxPossessRange; x++)
        {
            for (int z = -maxPossessRange; z <= maxPossessRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GridSystem.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxPossessRange) continue;

                Unit targetUnit = GridSystem.Instance.GetGridObject(testGridPosition).GetUnit();
                
                // Can only possess UNITS
                if (targetUnit == null) continue;
                
                // Can only possess ENEMIES
                if (targetUnit.IsEnemy == unit.IsEnemy) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override int GetActionPointsCost()
    {
        return 2; // Ends Turn
    }
}