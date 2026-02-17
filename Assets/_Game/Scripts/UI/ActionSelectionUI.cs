using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private ActionButtonUI actionButtonPrefab;

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
        foreach (Transform buttonTransform in actionButtonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;

        foreach (BaseAction action in selectedUnit.GetComponents<BaseAction>())
        {
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
