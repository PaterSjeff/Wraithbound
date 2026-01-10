using UnityEngine;

public class Unit : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private MeshRenderer unitMeshRenderer;
    [SerializeField] private MoveAction moveAction;

    [Header("Identity")]
    [SerializeField] private bool isGhost = false; 
    
    [Header("Stats")]
    [SerializeField] private BodyData_SO currentBodyData;

    private GridPosition gridPosition;
    private int currentStructure;

    // EDITOR ONLY: Auto-assigns components when you add the script!
    private void Reset()
    {
        unitMeshRenderer = GetComponentInChildren<MeshRenderer>();
        moveAction = GetComponent<MoveAction>();
    }

    private void Start()
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
    }

    private void Update()
    {
        GridPosition newGridPosition = GridSystem.Instance.GetGridPosition(transform.position);

        if (newGridPosition != gridPosition)
        {
            GridObject oldGridObject = GridSystem.Instance.GetGridObject(gridPosition);
            GridObject newGridObject = GridSystem.Instance.GetGridObject(newGridPosition);

            Unit targetUnit = newGridObject.GetUnit();
            if (targetUnit != null)
            {
                if (isGhost && !targetUnit.isGhost)
                {
                    PerformPossession(targetUnit);
                    return; 
                }
            }

            oldGridObject.SetUnit(null);
            newGridObject.SetUnit(this);
            gridPosition = newGridPosition;
        }
    }

    // Public getter avoids GetComponent in other scripts
    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public void PerformPossession(Unit targetBody)
    {
        // OPTIMIZED: Uses the cached reference, not GetComponent
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

        int minor = currentBodyData.defenseThresholds[0];
        int major = currentBodyData.defenseThresholds[1];
        int fatal = currentBodyData.defenseThresholds[2];

        if (incomingDamage >= fatal)
        {
            Debug.Log($"FATAL HIT! ({incomingDamage} vs {fatal})");
            currentStructure = 0;
        }
        else if (incomingDamage >= major)
        {
            Debug.Log($"CRIT! ({incomingDamage} vs {major})");
            currentStructure -= 2;
        }
        else if (incomingDamage >= minor)
        {
            Debug.Log($"HIT. ({incomingDamage} vs {minor})");
            currentStructure -= 1;
        }
        else
        {
            Debug.Log($"GLANCE. ({incomingDamage} < {minor})");
        }

        if (currentStructure <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GridPosition gridPos = GridSystem.Instance.GetGridPosition(transform.position);
        GridSystem.Instance.GetGridObject(gridPos).SetUnit(null);
        Destroy(gameObject);
    }
}