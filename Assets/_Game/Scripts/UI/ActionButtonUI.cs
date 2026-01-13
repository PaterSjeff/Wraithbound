using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedVisual; // Optional: Outline image

    private BaseAction action;

    public void SetBaseAction(BaseAction action)
    {
        this.action = action;
        textMeshPro.text = action.GetActionName().ToUpper();
        
        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(action);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        
        // Optional: Show a highlight if this is the currently selected action
        if (selectedVisual != null)
        {
            selectedVisual.SetActive(selectedAction == action);
        }
        
        // --------------------------------------------------------
        // THE LOGIC YOU ASKED FOR (Graying Out)
        // --------------------------------------------------------
        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
        
        // 1. Check Action Points (Has Moved? Turn Over?)
        bool canAfford = unit.CanSpendActionPointsToTakeAction(action);
        
        // 2. Check Targets (Is anyone in range?)
        // (Only strictly necessary for Attack/Possess, but safe for all)
        bool hasTargets = action.GetValidActionGridPositionList().Count > 0;

        // COMBINED CHECK
        if (canAfford && hasTargets)
        {
            button.interactable = true;
            //textMeshPro.color = Color.white; // Active Color
        }
        else
        {
            button.interactable = false;
            //textMeshPro.color = Color.gray; // Grayed Out
        }
    }
}