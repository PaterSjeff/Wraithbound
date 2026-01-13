using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("System References")]
    // Drag your Managers here in the Inspector!
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private UnitActionSystem unitActionSystem;

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
        if (inputManager == null || gridSystem == null || unitActionSystem == null)
        {
            Debug.LogError("GAME CRASH: You forgot to assign Managers in GameManager Inspector!");
            return;
        }

        Debug.Log("Booting Systems...");

        // 1. Inputs (Using the Inspector Reference)
        inputManager.Init();

        // 2. The Grid (Using the Inspector Reference)
        gridSystem.Init();

        // 3. The Unit System (Using the Inspector Reference)
        unitActionSystem.Init();
        
        Unit[] allUnits = FindObjectsOfType<Unit>();
        foreach (Unit unit in allUnits)
        {
            unit.Init();
        }
        
        Debug.Log("Boot Complete. Game Loop Started.");
    }
}