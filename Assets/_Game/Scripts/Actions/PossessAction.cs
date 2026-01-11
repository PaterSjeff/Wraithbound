using System;
using UnityEngine;

public class PossessAction : BaseAction
{
    [SerializeField] private int maxPossessRange = 1;

    public override string GetActionName()
    {
        return "Possess";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Unit targetUnit = GridSystem.Instance.GetGridObject(gridPosition).GetUnit();
        unit.PerformPossession(targetUnit);
        onActionComplete();
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        Unit targetUnit = GridSystem.Instance.GetGridObject(gridPosition).GetUnit();
        if (targetUnit == null)
        {
            // Must target a unit
            return false;
        }

        if (targetUnit.IsEnemy == unit.IsEnemy)
        {
            // Cannot possess friendly units
            return false;
        }

        int distance = gridPosition.Distance(unit.GetGridPosition());
        return distance <= maxPossessRange;
    }
}
