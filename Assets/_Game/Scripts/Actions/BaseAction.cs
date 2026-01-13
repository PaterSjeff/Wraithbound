using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    // Standardize 'isActive' vs 'isActionInProgress'
    protected bool isActive; 
    protected Unit unit;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    // --- CONTRACT METHODS (Must be implemented by children) ---
    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    
    // Default implementations (can be overridden)
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        return false;
    }

    public virtual List<GridPosition> GetValidActionGridPositionList()
    {
        return new List<GridPosition>();
    }

    public abstract int GetActionPointsCost();

    // --- HELPER METHODS ---
    protected void BaseActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        
        // Notify the System (for UI)
        UnitActionSystem.Instance.InvokeOnActionStarted(this);
    }

    protected void BaseActionComplete()
    {
        isActive = false;
        onActionComplete();

        // Notify the System (for UI)
        UnitActionSystem.Instance.InvokeOnActionCompleted(this);
    }
}