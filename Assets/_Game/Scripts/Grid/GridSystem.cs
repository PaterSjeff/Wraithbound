using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // Singleton pattern for easy access (as per TDD Arch Pattern)
    public static GridSystem Instance { get; private set; }

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 2f; // Keep spacing for isometric view

    private GridObject[,] gridObjects;
    
    [SerializeField] private Transform gridDebugObjectPrefab; // Assign your new prefab here

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("There's more than one GridSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CreateGrid();
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
                gridObjects[x, z] = new GridObject(this, gridPos);
                
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
}