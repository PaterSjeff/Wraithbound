using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private Button actionButtonPrefab;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        CreateActionButtons();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(Unit unit)
    {
        CreateActionButtons();
    }

    private void CreateActionButtons()
    {
        foreach (Transform button in actionButtonContainer)
        {
            Destroy(button.gameObject);
        }

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;

        foreach (BaseAction action in selectedUnit.GetComponents<BaseAction>())
        {
            if (action is PossessAction && !selectedUnit.IsGhost)
            {
                continue;
            }

            Button button = Instantiate(actionButtonPrefab, actionButtonContainer);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = action.GetActionName();
            button.onClick.AddListener(() =>
            {
                UnitActionSystem.Instance.SetSelectedAction(action);
            });
        }
    }
}
