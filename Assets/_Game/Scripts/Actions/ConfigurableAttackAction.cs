using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConfigurableAttackAction : BaseAction
{
    [SerializeField] private AttackData_SO attackData;

    public AttackData_SO AttackData => attackData;

    public void SetAttackData(AttackData_SO data)
    {
        attackData = data;
    }

    public override string GetActionName() => attackData != null ? attackData.attackName : "Attack";

    public override ActionResourceType GetActionResourceType()
    {
        if (attackData == null) return ActionResourceType.Attack;
        return attackData.costsAttackAction ? ActionResourceType.Attack : ActionResourceType.Ability;
    }

    public override bool CanExecute()
    {
        if (unit == null || attackData == null) return false;
        
        if (attackData.costsAttackAction)
            return unit.CanAttack();
        else
            return unit.CanSpendMana(attackData.manaCost);
    }

    public override int GetManaCost()
    {
        return attackData != null && !attackData.costsAttackAction ? attackData.manaCost : 0;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        if (attackData == null)
        {
            onActionComplete?.Invoke();
            return;
        }

        BaseActionStart(onActionComplete);
        StartCoroutine(ExecuteAttackCoroutine(gridPosition));
    }

    private IEnumerator ExecuteAttackCoroutine(GridPosition targetPosition)
    {
        // Get all affected positions based on targeting type
        List<GridPosition> affectedPositions = TargetingHelper.GetAffectedPositions(
            unit.GetGridPosition(),
            targetPosition,
            attackData.targetingType,
            attackData.range,
            attackData.aoeRadius
        );

        // Animation: Melee lunge or ranged attack
        if (attackData.isMeleeAnimation)
        {
            yield return MeleeAttackAnimation(targetPosition);
        }
        else
        {
            yield return RangedAttackAnimation(targetPosition);
        }

        // Deal damage to all affected targets
        int damage = attackData.useUnitAttackDamage ? unit.GetAttackDamage() : attackData.baseDamage;

        foreach (GridPosition pos in affectedPositions)
        {
            GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
            if (gridObj == null) continue;

            // Damage units
            Unit targetUnit = gridObj.GetUnit();
            if (targetUnit != null && targetUnit.IsEnemy != unit.IsEnemy)
            {
                targetUnit.TakeDamage(damage);
            }

            // Damage static objects
            StaticObject staticObj = gridObj.GetStaticObject();
            if (staticObj != null)
            {
                staticObj.TakeDamage(damage);
            }
        }

        BaseActionComplete();
    }

    private IEnumerator MeleeAttackAnimation(GridPosition targetPosition)
    {
        Vector3 startPos = transform.position;
        Vector3 targetWorldPos = GridSystem.Instance.GetWorldPosition(targetPosition);
        Vector3 direction = (targetWorldPos - startPos).normalized;
        Vector3 lungePos = startPos + direction * attackData.lungeDistance;

        transform.forward = direction;

        Sequence attackSeq = DOTween.Sequence();
        attackSeq.Append(transform.DOMove(lungePos, attackData.animationDuration * 0.4f).SetEase(Ease.OutQuad));
        attackSeq.AppendInterval(attackData.animationDuration * 0.2f);
        attackSeq.Append(transform.DOMove(startPos, attackData.animationDuration * 0.4f).SetEase(Ease.InQuad));

        yield return attackSeq.WaitForCompletion();
    }

    private IEnumerator RangedAttackAnimation(GridPosition targetPosition)
    {
        Vector3 targetWorldPos = GridSystem.Instance.GetWorldPosition(targetPosition);
        Vector3 direction = (targetWorldPos - transform.position).normalized;
        transform.forward = direction;

        // Simple wait for ranged attack animation
        // You can add projectile spawning here later
        yield return new WaitForSeconds(attackData.animationDuration);
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        if (attackData == null) return new List<GridPosition>();

        List<GridPosition> validPositions = TargetingHelper.GetValidTargetPositions(
            unit.GetGridPosition(),
            attackData.targetingType,
            attackData.range,
            attackData.minRange,
            attackData.aoeRadius
        );

        // Filter for enemy units based on targeting type
        List<GridPosition> validTargets = new List<GridPosition>();
        
        if (attackData.targetingType == TargetingType.Self)
        {
            validTargets.Add(unit.GetGridPosition());
            return validTargets;
        }

        foreach (GridPosition pos in validPositions)
        {
            GridObject gridObj = GridSystem.Instance.GetGridObject(pos);
            if (gridObj == null) continue;

            // For single target and line attacks, require an enemy unit
            if (attackData.targetingType == TargetingType.SingleTarget || 
                attackData.targetingType == TargetingType.Line)
            {
                Unit targetUnit = gridObj.GetUnit();
                if (targetUnit != null && targetUnit.IsEnemy != unit.IsEnemy)
                {
                    validTargets.Add(pos);
                }
            }
            // For AOE and Cone, allow targeting any valid position
            else if (attackData.targetingType == TargetingType.AoE || 
                     attackData.targetingType == TargetingType.Cone)
            {
                validTargets.Add(pos);
            }
        }

        return validTargets;
    }
}
