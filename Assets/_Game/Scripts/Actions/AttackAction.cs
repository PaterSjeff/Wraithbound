using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AttackAction : BaseAction
{
    [SerializeField] private int maxAttackRange = 1;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float lungeDistance = 0.5f;
    
    private Unit targetUnit;

    public override string GetActionName() => "Attack";

    public override ActionResourceType GetActionResourceType() => ActionResourceType.Attack;

    public override bool CanExecute() => unit != null && unit.CanAttack();

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        GridObject gridObject = GridSystem.Instance.GetGridObject(gridPosition);
        targetUnit = gridObject?.GetUnit();
        if (targetUnit == null) { onActionComplete?.Invoke(); return; }

        BaseActionStart(onActionComplete);
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = targetUnit.transform.position;
        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 lungePos = startPos + direction * lungeDistance;

        transform.forward = direction;

        Sequence attackSeq = DOTween.Sequence();
        attackSeq.Append(transform.DOMove(lungePos, attackDuration * 0.3f).SetEase(Ease.OutQuad));
        attackSeq.AppendCallback(() =>
        {
            int damage = unit.GetAttackDamage();
            targetUnit.TakeDamage(damage);
        });
        attackSeq.Append(transform.DOMove(startPos, attackDuration * 0.3f).SetEase(Ease.InQuad));

        yield return attackSeq.WaitForCompletion();
        BaseActionComplete();
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
        int range = unit.GetAttackRange();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GridSystem.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range) continue;

                // Must have a unit
                Unit targetUnit = GridSystem.Instance.GetGridObject(testGridPosition).GetUnit();
                if (targetUnit == null) continue;

                // Must be an enemy
                if (targetUnit.IsEnemy == unit.IsEnemy) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }
}