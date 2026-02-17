using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected bool isActive;
    protected Unit unit;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        return false;
    }

    public virtual List<GridPosition> GetValidActionGridPositionList()
    {
        return new List<GridPosition>();
    }

    public abstract ActionResourceType GetActionResourceType();

    public virtual bool CanExecute() => false;

    public virtual int GetManaCost() => 0;

    protected void BaseActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        UnitActionSystem.Instance.InvokeOnActionStarted(this);
    }

    protected void BaseActionComplete()
    {
        isActive = false;
        onActionComplete?.Invoke();
        UnitActionSystem.Instance.InvokeOnActionCompleted(this);
    }
}
