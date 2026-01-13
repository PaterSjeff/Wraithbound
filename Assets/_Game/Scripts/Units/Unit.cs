using UnityEngine;
using System;

public class Unit : MonoBehaviour, IDamageable
{
    // Event to update UI (e.g. gray out unit)
    public event Action OnActionPointsChanged; 

    [Header("References")]
    [SerializeField] private MeshRenderer unitMeshRenderer;
    
    [Header("Identity")]
    [SerializeField] private bool isGhost = false;
    public bool IsGhost => isGhost;
    [SerializeField] private bool isEnemy = false;
    public bool IsEnemy => isEnemy;
    
    [Header("Stats")]
    [SerializeField] private BodyData_SO currentBodyData;

    private GridPosition gridPosition;
    private int currentStructure;
    private BaseAction[] unitActions;

    // --- TURN STATE ---
    private bool hasMoved;
    private bool isTurnOver;

    private void Awake()
    {
        unitActions = GetComponents<BaseAction>();
    }

    public void Init()
    {
        gridPosition = GridSystem.Instance.GetGridPosition(transform.position);
        transform.position = GridSystem.Instance.GetWorldPosition(gridPosition);
        
        GridObject startNode = GridSystem.Instance.GetGridObject(gridPosition);
        if (startNode != null && startNode.GetUnit() == null)
        {
            startNode.SetUnit(this);
        }

        if (currentBodyData != null)
        {
            currentStructure = currentBodyData.maxStructure;
        }

        // CRITICAL: Listen for the turn to reset flags
        TurnManager.Instance.OnTurnChanged += TurnManager_OnTurnChanged;
    }

    private void Update()
    {
        GridPosition newGridPosition = GridSystem.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition)
        {
            GridObject oldGridObject = GridSystem.Instance.GetGridObject(gridPosition);
            GridObject newGridObject = GridSystem.Instance.GetGridObject(newGridPosition);

            oldGridObject.SetUnit(null);
            newGridObject.SetUnit(this);
            gridPosition = newGridPosition;
        }
    }

    // --- TURN LOGIC ---

    private void TurnManager_OnTurnChanged()
    {
        // If it is the Player Phase and I am a Player Unit -> Reset
        // If it is the Enemy Phase and I am an Enemy -> Reset
        if ((TurnManager.Instance.IsPlayerTurn() && !isEnemy) || 
            (!TurnManager.Instance.IsPlayerTurn() && isEnemy))
        {
            hasMoved = false;
            isTurnOver = false;
            OnActionPointsChanged?.Invoke();
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction action)
    {
        if (isTurnOver) return false;

        // RULE: You can only move once
        if (action is MoveAction)
        {
            if (hasMoved) return false; 
        }

        // RULE: You can always Attack/Possess if your turn isn't over
        return true; 
    }

    public void SpendActionPoints(BaseAction action)
    {
        if (action is MoveAction)
        {
            hasMoved = true;
        }
        else
        {
            // Any other action ends the turn
            isTurnOver = true;
        }
        OnActionPointsChanged?.Invoke();
    }
    
    // ---------------------

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction action in unitActions)
        {
            if (action is T) return (T)action;
        }
        return null;
    }

    public GridPosition GetGridPosition() => gridPosition;

    public void PerformPossession(Unit targetBody)
    {
        if (targetBody.unitMeshRenderer != null)
        {
            targetBody.unitMeshRenderer.material.color = Color.blue;
        }
        
        UnitActionSystem.Instance.SetSelectedUnit(targetBody);
        GridSystem.Instance.GetGridObject(gridPosition).SetUnit(null);
        Destroy(gameObject);
    }

    public void TakeDamage(int incomingDamage)
    {
        if (currentBodyData == null) return;
        currentStructure -= 1; // Simplified for brevity
        if (currentStructure <= 0) Die();
    }

    private void Die()
    {
        GridPosition gridPos = GridSystem.Instance.GetGridPosition(transform.position);
        GridSystem.Instance.GetGridObject(gridPos).SetUnit(null);
        Destroy(gameObject);
    }
}