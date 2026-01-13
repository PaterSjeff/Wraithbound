using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : BaseAction
{
    private enum State
    {
        Swinging,
        Cooldown,
    }

    [SerializeField] private int maxAttackRange = 1;
    [SerializeField] private int damageAmount = 1;
    
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    public override string GetActionName() => "Attack";

    private void Update()
    {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Swinging:
                float rotateSpeed = 10f;
                // Face the target
                Vector3 aimDirection = (targetUnit.transform.position - unit.transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, rotateSpeed * Time.deltaTime);
                break;
            case State.Cooldown:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Swinging:
                state = State.Cooldown;
                float cooldownTime = 0.5f;
                stateTimer = cooldownTime;
                
                // Hit the target
                targetUnit.TakeDamage(damageAmount);
                break;

            case State.Cooldown:
                BaseActionComplete();
                break;
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        GridObject gridObject = GridSystem.Instance.GetGridObject(gridPosition);
        targetUnit = gridObject.GetUnit();

        state = State.Swinging;
        float swingingTime = 0.7f;
        stateTimer = swingingTime;

        BaseActionStart(onActionComplete);
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

        for (int x = -maxAttackRange; x <= maxAttackRange; x++)
        {
            for (int z = -maxAttackRange; z <= maxAttackRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GridSystem.Instance.IsValidGridPosition(testGridPosition)) continue;

                // Math to check range (Manhattan or Euclidean)
                // Assuming GridPosition has a Distance or we use abs math
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxAttackRange) continue;

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

    public override int GetActionPointsCost()
    {
        return 2; // Ends Turn
    }
}