using System;
using UnityEngine;

public class AttackAction : BaseAction
{
    private enum State
    {
        Swinging,
        Cooldown,
    }

    [SerializeField] private int maxAttackRange = 1;
    
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActionInProgress) return;

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Swinging:
                float rotateSpeed = 10f;
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
                targetUnit.TakeDamage(10);
                break;
            case State.Cooldown:
                isActionInProgress = false;
                onActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Attack";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        targetUnit = GridSystem.Instance.GetGridObject(gridPosition).GetUnit();
        
        state = State.Swinging;
        float swingingTime = 0.7f;
        stateTimer = swingingTime;

        isActionInProgress = true;
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
            // Cannot attack friendly units
            return false;
        }

        int distance = gridPosition.Distance(unit.GetGridPosition());
        return distance <= maxAttackRange;
    }
}
