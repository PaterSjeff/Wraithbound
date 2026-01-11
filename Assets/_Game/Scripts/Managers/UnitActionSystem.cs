using System;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event Action<bool> OnBusyChanged;
    public event Action<Unit> OnSelectedUnitChanged;

    [SerializeField] private Unit selectedUnit; // Drag your Unit here manually for now
    private BaseAction selectedAction;
    private bool busy;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void Init()
    {
        // Any setup logic goes here. 
        // For now, it might be empty, but it ensures we are "Ready".
        Debug.Log("UnitActionSystem Ready.");
    }

    private void Update()
    {
        if (busy) return;
        if (!TurnManager.Instance.IsPlayerTurn()) return;

        // FIX: Use InputManager instead of Input.GetMouseButtonDown(0)
        if (InputManager.Instance.IsMouseButtonDown()) 
        {
            HandleUnitSelection();
        }
    }

    private void HandleUnitSelection()
    {
        if (TryGetGridPosition(out GridPosition gridPos))
        {
            if (selectedUnit != null && selectedAction != null)
            {
                if (selectedAction.IsValidActionGridPosition(gridPos))
                {
                    SetBusy(true);
                    selectedAction.TakeAction(gridPos, () => SetBusy(false));
                    return;
                }
            }

            GridObject gridObject = GridSystem.Instance.GetGridObject(gridPos);
            Unit targetUnit = gridObject.GetUnit();

            if (targetUnit != null)
            {
                if (!targetUnit.IsEnemy)
                {
                    SetSelectedUnit(targetUnit);
                }
            }
        }
    }
    
    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(unit);
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    private void SetBusy(bool isBusy)
    {
        busy = isBusy;
        OnBusyChanged?.Invoke(busy);
    }
    
    private bool TryGetGridPosition(out GridPosition gridPosition)
    {
        Vector3 mouseWorldPosition = MouseWorld.GetPosition();
        gridPosition = GridSystem.Instance.GetGridPosition(mouseWorldPosition);
        
        // Simple bounds check (optional but good practice)
        return GridSystem.Instance.GetGridObject(gridPosition) != null;
    }
}