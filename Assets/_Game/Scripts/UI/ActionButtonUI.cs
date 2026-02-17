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
        if (unit == null)
        {
            button.interactable = false;
            return;
        }

        bool canAfford = unit.CanSpendActionPointsToTakeAction(action);
        bool hasTargets = action.GetValidActionGridPositionList().Count > 0;

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