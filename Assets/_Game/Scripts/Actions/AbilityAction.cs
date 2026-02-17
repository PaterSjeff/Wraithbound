using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAction : BaseAction
{
    [SerializeField] private AbilityData_SO abilityData;

    public AbilityData_SO AbilityData => abilityData;

    public void SetAbilityData(AbilityData_SO data)
    {
        abilityData = data;
    }

    public override string GetActionName() => abilityData != null ? abilityData.abilityName : "Ability";

    public override ActionResourceType GetActionResourceType() => ActionResourceType.Ability;

    public override int GetManaCost() => abilityData != null ? abilityData.manaCost : 0;

    public override bool CanExecute()
    {
        return unit != null && abilityData != null && unit.CanSpendMana(abilityData.manaCost);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        if (abilityData == null) { onActionComplete?.Invoke(); return; }

        BaseActionStart(onActionComplete);

        var affected = TargetingHelper.GetAffectedPositions(
            unit.GetGridPosition(),
            gridPosition,
            abilityData.targetingType,
            abilityData.range,
            abilityData.aoeRadius);

        foreach (GridPosition pos in affected)
        {
            GridObject cell = GridSystem.Instance.GetGridObject(pos);
            if (cell == null) continue;

            Unit targetUnit = cell.GetUnit();
            if (targetUnit != null && targetUnit.IsEnemy != unit.IsEnemy)
            {
                targetUnit.TakeDamage(abilityData.damage);
                continue;
            }

            StaticObject staticObj = cell.GetStaticObject();
            if (staticObj != null)
                staticObj.TakeDamage(abilityData.damage);
        }

        BaseActionComplete();
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList().Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        if (abilityData == null || unit == null) return new List<GridPosition>();
        return TargetingHelper.GetValidTargetPositions(
            unit.GetGridPosition(),
            abilityData.targetingType,
            abilityData.range,
            0, // minRange
            abilityData.aoeRadius);
    }
}
