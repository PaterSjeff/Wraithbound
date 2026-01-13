using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    // Event fired whenever the Phase changes (e.g., Player -> Enemy)
    public event Action OnTurnChanged;

    public enum TurnPhase
    {
        Player,
        Enemy,
        Neutral // For "Other" units or Environmental effects later
    }

    private TurnPhase currentPhase = TurnPhase.Player;
    private int turnNumber = 1;

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
        // Optional: Ensure we start correctly
        OnTurnChanged?.Invoke();
    }

    public void NextTurn()
    {
        // Cycle the phases: Player -> Enemy -> Neutral -> Player
        switch (currentPhase)
        {
            case TurnPhase.Player:
                currentPhase = TurnPhase.Enemy;
                StartCoroutine(EnemyTurnRoutine());
                break;
                
            case TurnPhase.Enemy:
                currentPhase = TurnPhase.Neutral;
                StartCoroutine(NeutralTurnRoutine());
                break;

            case TurnPhase.Neutral:
                turnNumber++;
                currentPhase = TurnPhase.Player;
                break;
        }

        Debug.Log($"Phase Changed: {currentPhase} (Turn {turnNumber})");
        OnTurnChanged?.Invoke();
    }

    private IEnumerator EnemyTurnRoutine()
    {
        // Wait 2 seconds as requested (simulating "Thinking" time)
        yield return new WaitForSeconds(2f);
        
        // In the future, this is where you would call: EnemyAI.Instance.TakeAction();
        // For now, it just skips back to the next phase.
        NextTurn();
    }

    private IEnumerator NeutralTurnRoutine()
    {
        // Short pause for "Other" units
        yield return new WaitForSeconds(0.5f);
        NextTurn();
    }

    // Helper for UI and Input blocking
    public bool IsPlayerTurn()
    {
        return currentPhase == TurnPhase.Player;
    }

    // Helper for AI logic later
    public bool IsEnemyTurn()
    {
        return currentPhase == TurnPhase.Enemy;
    }

    public int GetTurnNumber() => turnNumber;
}