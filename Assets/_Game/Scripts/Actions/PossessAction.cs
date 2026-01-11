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
        // Visual flair: Orient towards the target before possessing
        transform.LookAt(GridSystem.Instance.GetWorldPosition(gridPosition)); 
        unit.PerformPossession(targetUnit);
        onActionComplete();
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        // 1. Check if the tile is valid
        if (!GridSystem.Instance.IsValidGridPosition(gridPosition)) return false;

        GridObject gridObject = GridSystem.Instance.GetGridObject(gridPosition);
        Unit targetUnit = gridObject.GetUnit();

        // 2. Check if there is a unit there
        if (targetUnit == null)
        {
            // Valid behavior: Don't log here (it spams for empty tiles)
            return false;
        }

        // 3. Team Check (This is the likely error)
        if (targetUnit.IsEnemy == unit.IsEnemy)
        {
            Debug.Log($"Possess Failed: Target {targetUnit.name} is on the same team ({unit.IsEnemy}).");
            return false;
        }

        // 4. Distance Check (Manhattan vs Diagonal?)
        // Note: Make sure your GridPosition struct has a valid Distance check!
        // If using Manhattan distance, diagonals = 2 (Invalid for Range 1).
        // Try increasing Range to 2 in Inspector to test.
        int distance = Math.Abs(gridPosition.x - unit.GetGridPosition().x) + 
                       Math.Abs(gridPosition.z - unit.GetGridPosition().z);
        
        if (distance > maxPossessRange)
        {
            Debug.Log($"Possess Failed: Target too far ({distance} > {maxPossessRange})");
            return false;
        }

        return true;
    }
}