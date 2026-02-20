using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("System References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private UnitActionSystem unitActionSystem;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Pathfinding pathfinding;
    [SerializeField] private TileHighlightManager tileHighlightManager;
    [SerializeField] private TilePainter tilePainter;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // VALIDATION: Fail fast if you forgot to drag them in
        if (inputManager == null || gridSystem == null || unitActionSystem == null || turnManager == null || pathfinding == null)
        {
            Debug.LogError("GAME CRASH: You forgot to assign Managers in GameManager Inspector!");
            return;
        }
        
        if (tileHighlightManager == null)
        {
            Debug.LogWarning("TileHighlightManager not assigned - tile highlights will not work!");
        }

        Debug.Log("Booting Systems...");

        // 1. Inputs (Using the Inspector Reference)
        inputManager.Init();

        // 2. The Grid (Using the Inspector Reference)
        gridSystem.Init();
        pathfinding.Init();

        // Paint tiles after grid is initialized
        if (tilePainter != null)
            tilePainter.PaintTiles();

        foreach (StaticObject so in FindObjectsOfType<StaticObject>())
            so.Init(gridSystem.GetGridPosition(so.transform.position));

        // Initialize tile highlighting AFTER grid debug objects are spawned
        if (tileHighlightManager != null)
            tileHighlightManager.Init();

        // 3. The Unit System (Using the Inspector Reference)
        unitActionSystem.Init();
        
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            unit.Init();
        }

        turnManager.OnCombatEnded += OnCombatEnded;
        turnManager.StartFirstRound();

        Debug.Log("Boot Complete. Game Loop Started.");
    }

    private void OnCombatEnded(bool playerWon)
    {
        Debug.Log(playerWon ? "Combat won!" : "Combat lost.");
    }
}