using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool isGhost = false; // Check this box for your Ghost Sphere
    private GridPosition gridPosition;
    [SerializeField] private MeshRenderer meshRenderer;

    private void Start()
    {
        // 1. Calculate where I am starting using the SYSTEM, not the Object
        gridPosition = GridSystem.Instance.GetGridPosition(transform.position);

        // 2. Snap to the center of that tile visually
        transform.position = GridSystem.Instance.GetWorldPosition(gridPosition);
        
        // 3. Register self (only if the tile is empty)
        GridObject startNode = GridSystem.Instance.GetGridObject(gridPosition);
        
        // Safety Check: Ensure the node actually exists before accessing it
        if (startNode != null && startNode.GetUnit() == null)
        {
            startNode.SetUnit(this);
        }
    }

    private void Update()
    {
        GridPosition newGridPosition = GridSystem.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition)
        {
            GridObject oldGridObject = GridSystem.Instance.GetGridObject(gridPosition);
            GridObject newGridObject = GridSystem.Instance.GetGridObject(newGridPosition);

            // 1. Check for Collision / Possession
            Unit targetUnit = newGridObject.GetUnit();
            if (targetUnit != null)
            {
                if (isGhost && !targetUnit.isGhost)
                {
                    // POSSESSION LOGIC
                    PerformPossession(targetUnit);
                    return; // Stop execution so we don't register the dead ghost to the tile
                }
            }

            // 2. Standard Movement Logic
            oldGridObject.SetUnit(null);
            newGridObject.SetUnit(this);
            gridPosition = newGridPosition;
        }
    }

    private void PerformPossession(Unit targetBody)
    {
        // A. Visuals: Turn the enemy Blue
        meshRenderer.material.color = Color.blue;
        
        // B. Logic: Select the new body
        UnitActionSystem.Instance.SetSelectedUnit(targetBody);

        // C. Cleanup: Destroy the Ghost
        // (Clear the old tile first so it doesn't stay "occupied" by a dead ghost)
        GridSystem.Instance.GetGridObject(gridPosition).SetUnit(null);
        Destroy(gameObject);
    }
}