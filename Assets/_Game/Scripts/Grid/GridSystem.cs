using UnityEngine;
using NaughtyAttributes;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f;

    private GridObject[,] gridObjects;

    [SerializeField] private Transform gridDebugObjectPrefab;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

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
        CreateGrid();
        Debug.Log("GridSystem Initialized.");
    }

    private void CreateGrid()
    {
        gridObjects = new GridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPos = new GridPosition(x, z);
                GridObject gridObject = new GridObject(this, gridPos);
                gridObjects[x, z] = gridObject;

                Transform debugTransform = Instantiate(gridDebugObjectPrefab, GetWorldPosition(gridPos), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(gridObjects[x, z]);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(
            gridPosition.x * cellSize + cellSize * 0.5f,
            0f,
            gridPosition.z * cellSize + cellSize * 0.5f
        );
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.FloorToInt(worldPosition.x / cellSize),
            Mathf.FloorToInt(worldPosition.z / cellSize)
        );
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        if (IsValidGridPosition(gridPosition))
            return gridObjects[gridPosition.x, gridPosition.z];
        return null;
    }

    public StaticObject GetStaticObject(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridObject(gridPosition);
        return gridObject?.GetStaticObject();
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.z >= 0 &&
               gridPosition.x < width &&
               gridPosition.z < height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = Vector3.zero;
        float w = width * cellSize;
        float h = height * cellSize;

        for (int x = 0; x <= width; x++)
        {
            float px = x * cellSize;
            Gizmos.DrawLine(origin + new Vector3(px, 0, 0), origin + new Vector3(px, 0, h));
        }

        for (int z = 0; z <= height; z++)
        {
            float pz = z * cellSize;
            Gizmos.DrawLine(origin + new Vector3(0, 0, pz), origin + new Vector3(w, 0, pz));
        }
    }
}
