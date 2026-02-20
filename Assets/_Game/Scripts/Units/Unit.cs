using UnityEngine;
using System;
using DG.Tweening;

public class Unit : MonoBehaviour, IDamageable
{
    public event Action OnActionPointsChanged;

    [Header("References")]
    [SerializeField] private MeshRenderer unitMeshRenderer;

    [Header("Damage Flash Settings")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float damageFlashDuration = 0.15f;
    [SerializeField] private float damagePunchScale = 0.1f;
    [SerializeField] private float damagePunchDuration = 0.25f;

    [Header("Identity")]
    [SerializeField] private bool isEnemy = false;
    public bool IsEnemy => isEnemy;

    [Header("Data (overrides serialized stats when set)")]
    [SerializeField] private UnitData_SO unitData;

    [Header("Stats (used when UnitData is not set)")]
    [SerializeField] private int maxStructure = 3;
    [SerializeField] private int speed = 10;
    [SerializeField] private int turnsPerRound = 1;
    [SerializeField] private int movesPerTurn = 1;
    [SerializeField] private int attacksPerTurn = 1;
    [SerializeField] private int maxMana = 5;

    private GridPosition gridPosition;
    private int currentHP;
    private int currentMana;
    private BaseAction[] unitActions;
    private int movesRemaining;
    private int attacksRemaining;

    public UnitData_SO UnitData => unitData;
    public int CurrentHP => currentHP;

    private void Awake()
    {
        unitActions = GetComponents<BaseAction>();
        
        // Auto-find MeshRenderer if not assigned
        if (unitMeshRenderer == null)
            unitMeshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Init()
    {
        gridPosition = GridSystem.Instance.GetGridPosition(transform.position);
        transform.position = GridSystem.Instance.GetWorldPosition(gridPosition);

        GridObject startNode = GridSystem.Instance.GetGridObject(gridPosition);
        if (startNode != null && startNode.GetUnit() == null)
            startNode.SetUnit(this);

        int maxHp = unitData != null ? unitData.maxHP : maxStructure;
        int maxM = unitData != null ? unitData.maxMana : maxMana;
        currentHP = maxHp;
        currentMana = maxM;

        // Initialize actions from UnitData if available
        if (unitData != null)
        {
            UnitActionInitializer.InitializeActions(this);
            unitActions = GetComponents<BaseAction>();
        }
    }

    public void StartTurn()
    {
        int moves = unitData != null ? unitData.movesPerTurn : movesPerTurn;
        int attacks = unitData != null ? unitData.attacksPerTurn : attacksPerTurn;
        movesRemaining = moves;
        attacksRemaining = attacks;
        OnActionPointsChanged?.Invoke();
        
        // Apply per-turn tile effects
        if (TileEffectManager.Instance != null)
            TileEffectManager.Instance.ApplyTileEffectsPerTurn(this);
    }

    public bool CanMove() => movesRemaining > 0;
    public bool CanAttack() => attacksRemaining > 0;
    public void SpendMove() { movesRemaining--; OnActionPointsChanged?.Invoke(); }
    public void SpendAttack() { attacksRemaining--; OnActionPointsChanged?.Invoke(); }

    public int GetCurrentMana() => currentMana;
    public bool CanSpendMana(int amount) => currentMana >= amount;
    public void SpendMana(int amount) { currentMana -= amount; OnActionPointsChanged?.Invoke(); }

    public int GetSpeed() => unitData != null ? unitData.speed : speed;
    public int GetTurnsPerRound() => unitData != null ? unitData.turnsPerRound : turnsPerRound;
    public int GetMoveRange() => unitData != null ? unitData.moveRange : 4;
    public int GetAttackRange() => unitData != null ? unitData.attackRange : 1;
    public int GetAttackDamage() => unitData != null ? unitData.attackDamage : 1;

    private void Update()
    {
        GridPosition newGridPosition = GridSystem.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridObject oldGridObject = GridSystem.Instance.GetGridObject(gridPosition);
            GridObject newGridObject = GridSystem.Instance.GetGridObject(newGridPosition);
            oldGridObject?.SetUnit(null);
            newGridObject?.SetUnit(this);
            gridPosition = newGridPosition;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction action)
    {
        return action != null && action.CanExecute();
    }

    public void SpendActionPoints(BaseAction action)
    {
        if (action == null) return;
        var resourceType = action.GetActionResourceType();
        if (resourceType == ActionResourceType.Move) SpendMove();
        else if (resourceType == ActionResourceType.Attack) SpendAttack();
        else if (resourceType == ActionResourceType.Ability && action.GetManaCost() > 0) SpendMana(action.GetManaCost());
        OnActionPointsChanged?.Invoke();
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction action in unitActions)
        {
            if (action is T) return (T)action;
        }
        return null;
    }

    public BaseAction[] GetActions() => unitActions;

    public GridPosition GetGridPosition() => gridPosition;

    public void SetGridPosition(GridPosition newPos)
    {
        GridObject oldCell = GridSystem.Instance.GetGridObject(gridPosition);
        GridObject newCell = GridSystem.Instance.GetGridObject(newPos);
        oldCell?.SetUnit(null);
        newCell?.SetUnit(this);
        gridPosition = newPos;
        transform.position = GridSystem.Instance.GetWorldPosition(newPos);
        
        // Apply tile effects when entering a new tile
        if (TileEffectManager.Instance != null)
            TileEffectManager.Instance.ApplyTileEffectsOnEnter(this, newPos);
    }

    public void TakeDamage(int incomingDamage)
    {
        currentHP -= incomingDamage;
        
        // Color flash
        if (unitMeshRenderer != null && unitMeshRenderer.material != null)
        {
            Material mat = unitMeshRenderer.material;
            Color originalColor = mat.color;
            
            mat.DOKill();
            
            Sequence damageSeq = DOTween.Sequence();
            damageSeq.Append(mat.DOColor(damageFlashColor, damageFlashDuration));
            damageSeq.Append(mat.DOColor(originalColor, damageFlashDuration));
        }
        
        // Scale punch
        if (damagePunchScale > 0)
        {
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * damagePunchScale, damagePunchDuration, 5, 0.5f);
        }
        
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        GridObject gridObj = GridSystem.Instance.GetGridObject(gridPosition);
        gridObj?.SetUnit(null);
        Destroy(gameObject);
    }
}
