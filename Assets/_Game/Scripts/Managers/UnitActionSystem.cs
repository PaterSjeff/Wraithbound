using System;
using UnityEngine;
using UnityEngine.EventSystems; // Needed for UI blocking

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    // --- EVENTS FOR UI ---
    public event Action<Unit> OnSelectedUnitChanged;
    public event Action<BaseAction> OnSelectedActionChanged;
    public event Action<bool> OnBusyChanged;
    
    // Events with standard (sender, args) signature for the UI script
    public event EventHandler OnActionStarted;
    public event EventHandler OnActionCompleted;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void Init() { }

    private void Update()
    {
        if (isBusy) return;
        if (!TurnManager.Instance.IsPlayerTurn()) return;
        
        // Block clicks if hovering over UI (Buttons)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (InputManager.Instance.IsMouseButtonDown())
        {
            HandleUnitSelection();
        }
    }

    private void HandleUnitSelection()
    {
        // 1. TRY TO MOVE/ACT
        GridPosition mouseGridPosition = GridSystem.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (selectedUnit != null && selectedAction != null)
        {
            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                // Can we afford it?
                if (selectedUnit.CanSpendActionPointsToTakeAction(selectedAction))
                {
                    SetBusy();
                    selectedUnit.SpendActionPoints(selectedAction);
                    selectedAction.TakeAction(mouseGridPosition, ClearBusy);
                }
                return;
            }
        }

        // 2. TRY TO SELECT A UNIT
        if (TryHandleUnitSelection()) return;
    }

    private bool TryHandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {
                if (unit == selectedUnit) return false;
                if (unit.IsEnemy) return false;

                SetSelectedUnit(unit);
                return true;
            }
        }
        return false;
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        // Default to Move Action when selecting a new unit
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(unit);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(baseAction);
    }

    public Unit GetSelectedUnit() => selectedUnit;
    public BaseAction GetSelectedAction() => selectedAction;

    // --- EVENT HELPERS ---
    public void InvokeOnActionStarted(BaseAction action)
    {
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeOnActionCompleted(BaseAction action)
    {
        OnActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(isBusy);
    }
}