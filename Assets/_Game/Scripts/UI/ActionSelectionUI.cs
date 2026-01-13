using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private ActionButtonUI actionButtonPrefab; // CHANGED: Now references the Script, not just Button

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        UnitActionSystem.Instance.OnActionCompleted += UnitActionSystem_OnActionCompleted;
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
        
        CreateActionButtons();
    }

    private void CreateActionButtons()
    {
        // Cleanup old buttons
        foreach (Transform buttonTransform in actionButtonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;

        // Create new buttons
        foreach (BaseAction action in selectedUnit.GetComponents<BaseAction>())
        {
            // Logic: Ghosts can only Move or Possess
            if (selectedUnit.IsGhost)
            {
                // If it's not Move or Possess, skip it
                if (!(action is MoveAction) && !(action is PossessAction)) continue;
            }
            // Logic: Bodies cannot Possess
            else 
            {
                 if (action is PossessAction) continue;
            }

            // Spawn
            ActionButtonUI actionButtonUI = Instantiate(actionButtonPrefab, actionButtonContainer);
            actionButtonUI.SetBaseAction(action);
            
            actionButtonUIList.Add(actionButtonUI);
        }

        UpdateActionButtons();
    }

    private void UpdateActionButtons()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    // --- EVENTS ---

    private void UnitActionSystem_OnSelectedUnitChanged(Unit unit)
    {
        CreateActionButtons();
    }

    private void UnitActionSystem_OnSelectedActionChanged(BaseAction action)
    {
        UpdateActionButtons();
    }

    private void UnitActionSystem_OnActionStarted(object sender, System.EventArgs e)
    {
        UpdateActionButtons();
    }

    private void UnitActionSystem_OnActionCompleted(object sender, System.EventArgs e)
    {
        UpdateActionButtons();
    }
    
    private void TurnManager_OnTurnChanged()
    {
        UpdateActionButtons();
    }
}