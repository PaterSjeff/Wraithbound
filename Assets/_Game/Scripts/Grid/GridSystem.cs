using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // Singleton pattern for easy access (as per TDD Arch Pattern)
    public static GridSystem Instance { get; private set; }

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f; // Keep spacing for isometric view
    
    // NEW: The layer we check for walls
    [SerializeField] private LayerMask obstaclesLayerMask;

    private GridObject[,] gridObjects;
    
    [SerializeField] private Transform gridDebugObjectPrefab; // Assign your new prefab here

    private void Awake()
    {
        // Singleton Only
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
                // Create the data container for this tile
                GridPosition gridPos = new GridPosition(x, z);
                GridObject gridObject = new GridObject(this, gridPos);
                gridObjects[x, z] = gridObject;
                
                // We cast a ray from high up (World Y + 100) straight down.
                // If it hits anything on the 'Obstacles' layer, this tile is NOT walkable.
                Vector3 worldPosition = GetWorldPosition(gridPos);
                float raycastOffsetDistance = 5f;
                
                // Note: We scan from the center of the cell to avoid edge cases
                if (Physics.Raycast(
                        worldPosition + Vector3.down * raycastOffsetDistance, 
                        Vector3.up, 
                        raycastOffsetDistance * 2, 
                        obstaclesLayerMask))
                {
                    gridObject.SetIsWalkable(false);
                }
                
                Transform debugTransform = Instantiate(gridDebugObjectPrefab, GetWorldPosition(gridPos), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(gridObjects[x, z]);
                
                // TODO: Spawn Debug Text here (TDD Section 6, Item 1)
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }
    
    public GridObject GetGridObject(GridPosition gridPosition)
    {
        if (IsValidGridPosition(gridPosition))
        {
            return gridObjects[gridPosition.x, gridPosition.z];
        }
        
        // Return null silently (preferable for gameplay checks) or log error if critical
        return null;
    }
    
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.z >= 0 && 
               gridPosition.x < width && 
               gridPosition.z < height;
    }
}