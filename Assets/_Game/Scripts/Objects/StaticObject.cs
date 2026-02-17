using UnityEngine;

public class StaticObject : MonoBehaviour, IDamageable
{
    [SerializeField] private StaticObjectData_SO data;

    private int currentHP;
    private GridPosition gridPosition;
    private bool initialized;

    public StaticObjectData_SO Data => data;
    public GridPosition GridPosition => gridPosition;
    public bool IsPushable => data != null && data.isPushable;

    public void Init(GridPosition pos)
    {
        if (initialized) return;
        gridPosition = pos;
        if (data != null)
            currentHP = data.maxHP;
        Vector3 worldPos = GridSystem.Instance.GetWorldPosition(gridPosition);
        transform.position = worldPos;
        GridObject gridObject = GridSystem.Instance.GetGridObject(gridPosition);
        if (gridObject != null)
            gridObject.SetStaticObject(this);
        initialized = true;
    }

    public void SetGridPosition(GridPosition newPos)
    {
        GridObject oldCell = GridSystem.Instance.GetGridObject(gridPosition);
        if (oldCell != null)
            oldCell.SetStaticObject(null);

        gridPosition = newPos;
        GridObject newCell = GridSystem.Instance.GetGridObject(gridPosition);
        if (newCell != null)
            newCell.SetStaticObject(this);
    }

    public void TakeDamage(int rawDamageAmount)
    {
        currentHP -= rawDamageAmount;
        if (currentHP <= 0)
            DestroyObject();
    }

    private void DestroyObject()
    {
        GridObject gridObject = GridSystem.Instance.GetGridObject(gridPosition);
        if (gridObject != null)
            gridObject.SetStaticObject(null);
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return data != null ? data.objectName : "StaticObject";
    }
}
