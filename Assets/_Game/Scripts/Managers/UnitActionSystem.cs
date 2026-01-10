using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] private Unit selectedUnit; // Drag your Unit here manually for now

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // FIX: Use InputManager instead of Input.GetMouseButtonDown(0)
        if (InputManager.Instance.IsMouseButtonDown()) 
        {
            Vector3 mouseWorldPosition = MouseWorld.GetPosition();
            GridPosition gridPosition = GridSystem.Instance.GetGridPosition(mouseWorldPosition);
            
            // ... rest of the code remains the same ...
            Vector3 gridWorldPosition = GridSystem.Instance.GetWorldPosition(gridPosition);
            if (selectedUnit != null)
            {
                selectedUnit.GetComponent<MoveAction>().Move(gridWorldPosition);
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
}