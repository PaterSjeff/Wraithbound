using System.Collections.Generic;
using UnityEngine;

public class TileHighlightManager : MonoBehaviour
{
    public static TileHighlightManager Instance { get; private set; }

    [SerializeField] private Color moveHighlightColor = new Color(0.3f, 0.6f, 1f, 0.5f);
    [SerializeField] private Color attackHighlightColor = new Color(1f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color abilityHighlightColor = new Color(0.8f, 0.3f, 1f, 0.5f);
    
    [Header("Hover Preview Colors")]
    [SerializeField] private Color hoverMoveColor = new Color(0.5f, 0.8f, 1f, 0.7f);
    [SerializeField] private Color hoverAttackColor = new Color(1f, 0.5f, 0.5f, 0.7f);
    [SerializeField] private Color hoverAbilityColor = new Color(1f, 0.5f, 1f, 0.7f);

    private Dictionary<GridPosition, GridDebugObject> gridDebugObjects = new Dictionary<GridPosition, GridDebugObject>();
    private GridPosition lastHoverPosition = new GridPosition(-999, -999);

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
        GridDebugObject[] debugObjects = FindObjectsOfType<GridDebugObject>();
        
        foreach (GridDebugObject debugObj in debugObjects)
        {
            GridPosition pos = GridSystem.Instance.GetGridPosition(debugObj.transform.position);
            gridDebugObjects[pos] = debugObj;
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
        TurnManager.Instance.OnTurnChanged += ClearAllHighlights;
    }

    private void Update()
    {
        UpdateHoverPreview();
    }

    private void UpdateHoverPreview()
    {
        BaseAction selectedAction = UnitActionSystem.Instance?.GetSelectedAction();
        if (selectedAction == null || !TurnManager.Instance.IsPlayerTurn())
        {
            ClearHoverPreview();
            return;
        }

        GridPosition hoverPosition = GridSystem.Instance.GetGridPosition(MouseWorld.GetPosition());
        
        // Only update if hover position changed
        if (hoverPosition.Equals(lastHoverPosition))
            return;
        
        lastHoverPosition = hoverPosition;
        ClearHoverPreview();

        // Check if this is a valid target position
        if (!selectedAction.IsValidActionGridPosition(hoverPosition))
            return;

        // Get all positions that would be affected by this action
        List<GridPosition> affectedPositions = GetAffectedPositionsForAction(selectedAction, hoverPosition);
        Color hoverColor = GetHoverColorForAction(selectedAction);

        foreach (GridPosition pos in affectedPositions)
        {
            GridDebugObject debugObj = GetGridDebugObject(pos);
            debugObj?.SetHoverPreview(true, hoverColor);
        }
    }

    private List<GridPosition> GetAffectedPositionsForAction(BaseAction action, GridPosition targetPosition)
    {
        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
        if (unit == null) return new List<GridPosition>();

        // For MoveAction, just return the target position
        if (action is MoveAction)
        {
            return new List<GridPosition> { targetPosition };
        }

        // For ConfigurableAttackAction, use its targeting data
        if (action is ConfigurableAttackAction configAttack && configAttack.AttackData != null)
        {
            return TargetingHelper.GetAffectedPositions(
                unit.GetGridPosition(),
                targetPosition,
                configAttack.AttackData.targetingType,
                configAttack.AttackData.range,
                configAttack.AttackData.aoeRadius
            );
        }

        // For AbilityAction, use its targeting data
        if (action is AbilityAction ability && ability.AbilityData != null)
        {
            return TargetingHelper.GetAffectedPositions(
                unit.GetGridPosition(),
                targetPosition,
                ability.AbilityData.targetingType,
                ability.AbilityData.range,
                ability.AbilityData.aoeRadius
            );
        }

        // For legacy AttackAction, just return target position
        if (action is AttackAction)
        {
            return new List<GridPosition> { targetPosition };
        }

        return new List<GridPosition> { targetPosition };
    }

    private GridDebugObject GetGridDebugObject(GridPosition pos)
    {
        gridDebugObjects.TryGetValue(pos, out GridDebugObject debugObj);
        return debugObj;
    }

    private void OnSelectedActionChanged(BaseAction action)
    {
        ClearAllHighlights();
        ClearHoverPreview();
        lastHoverPosition = new GridPosition(-999, -999);
        
        if (action == null) return;

        List<GridPosition> validPositions = action.GetValidActionGridPositionList();
        Color color = GetColorForAction(action);

        foreach (GridPosition pos in validPositions)
        {
            GridDebugObject debugObj = GetGridDebugObject(pos);
            debugObj?.SetHighlight(true, color);
        }
    }

    private void OnSelectedUnitChanged(Unit unit)
    {
        ClearAllHighlights();
        ClearHoverPreview();
        lastHoverPosition = new GridPosition(-999, -999);
    }

    public void ClearHighlights()
    {
        ClearAllHighlights();
    }

    public void ClearHover()
    {
        ClearHoverPreview();
        lastHoverPosition = new GridPosition(-999, -999);
    }

    private void ClearAllHighlights()
    {
        foreach (var kvp in gridDebugObjects)
            kvp.Value?.ClearHighlight();
    }

    private void ClearHoverPreview()
    {
        foreach (var kvp in gridDebugObjects)
            kvp.Value?.ClearHoverPreview();
    }

    private Color GetColorForAction(BaseAction action)
    {
        ActionResourceType resourceType = action.GetActionResourceType();
        return resourceType switch
        {
            ActionResourceType.Move => moveHighlightColor,
            ActionResourceType.Attack => attackHighlightColor,
            ActionResourceType.Ability => abilityHighlightColor,
            _ => Color.white
        };
    }

    private Color GetHoverColorForAction(BaseAction action)
    {
        ActionResourceType resourceType = action.GetActionResourceType();
        return resourceType switch
        {
            ActionResourceType.Move => hoverMoveColor,
            ActionResourceType.Attack => hoverAttackColor,
            ActionResourceType.Ability => hoverAbilityColor,
            _ => Color.white
        };
    }
}
