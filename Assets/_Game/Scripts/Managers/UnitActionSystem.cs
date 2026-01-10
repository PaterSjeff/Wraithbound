using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] private Unit selectedUnit; // Drag your Unit here manually for now

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    // Remove Start()!

    public void Init()
    {
        // Any setup logic goes here. 
        // For now, it might be empty, but it ensures we are "Ready".
        Debug.Log("UnitActionSystem Ready.");
    }

    private void Update()
    {
        // FIX: Use InputManager instead of Input.GetMouseButtonDown(0)
        if (InputManager.Instance.IsMouseButtonDown()) 
        {
            if (TryGetGridPosition(out GridPosition gridPos))
            {
                if (selectedUnit != null)
                {
                    Vector3 worldPos = GridSystem.Instance.GetWorldPosition(gridPos);
                
                    // OPTIMIZED: No GetComponent here anymore!
                    selectedUnit.GetMoveAction().Move(worldPos);
                }
            }
        }
        
        if (InputManager.Instance.IsRightMouseButtonDown()) 
        {
            if (TryGetGridPosition(out GridPosition gridPos))
            {
                GridObject gridObject = GridSystem.Instance.GetGridObject(gridPos);
                Unit targetUnit = gridObject.GetUnit();

                if (targetUnit != null)
                {
                    // Roll random damage (5-25) to test Glance/Hit/Crit
                    int roll = Random.Range(5, 25);
                    Debug.Log($"[DEBUG] Attacking {targetUnit.name} with Roll: {roll}");
                    targetUnit.TakeDamage(roll);
                }
            }
        }
    }
    
    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        
        // Visual Feedback (Optional but helpful)
        Debug.Log($"Possessed: {unit.name}");
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
    
    private bool TryGetGridPosition(out GridPosition gridPosition)
    {
        Vector3 mouseWorldPosition = MouseWorld.GetPosition();
        gridPosition = GridSystem.Instance.GetGridPosition(mouseWorldPosition);
        
        // Simple bounds check (optional but good practice)
        return GridSystem.Instance.GetGridObject(gridPosition) != null;
    }
}