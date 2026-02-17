using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    /// <summary>Fired when the active unit changes (whose turn it is).</summary>
    public event Action<Unit> OnActiveUnitChanged;

    /// <summary>Fired when turn state changes (for UI refresh).</summary>
    public event Action OnTurnChanged;

    /// <summary>Fired when a new round starts.</summary>
    public event Action OnRoundStarted;

    /// <summary>Fired when combat ends. True = player won, false = player lost.</summary>
    public event Action<bool> OnCombatEnded;

    private List<Unit> initiativeQueue = new List<Unit>();
    private int queueIndex;
    private Unit activeUnit;
    private int roundNumber = 1;
    private bool combatEnded;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>Call after all units are in the scene and initialized (e.g. from GameManager).</summary>
    public void StartFirstRound()
    {
        combatEnded = false;
        roundNumber = 1;
        BuildInitiativeQueue();
        OnRoundStarted?.Invoke();
        AdvanceToNextTurn();
    }

    /// <summary>Player or AI calls this when the current unit is done with their turn.</summary>
    public void RequestEndTurn()
    {
        AdvanceToNextTurn();
    }

    public Unit GetActiveUnit() => activeUnit;

    /// <summary>True when it is a player-controlled unit's turn (waiting for input).</summary>
    public bool IsPlayerTurn()
    {
        return activeUnit != null && !activeUnit.IsEnemy;
    }

    public bool IsEnemyTurn()
    {
        return activeUnit != null && activeUnit.IsEnemy;
    }

    public int GetRoundNumber() => roundNumber;

    /// <summary>Ordered list of units in initiative order for this round (for UI).</summary>
    public IReadOnlyList<Unit> GetInitiativeOrder() => initiativeQueue;

    /// <summary>Current position in the initiative queue (for UI).</summary>
    public int GetCurrentQueueIndex() => queueIndex;

    private void BuildInitiativeQueue()
    {
        initiativeQueue.Clear();
        Unit[] allUnits = FindObjectsOfType<Unit>();
        var entries = new List<(Unit unit, int turnIndex)>();
        foreach (Unit u in allUnits)
        {
            int turns = Mathf.Max(1, u.GetTurnsPerRound());
            for (int i = 0; i < turns; i++)
                entries.Add((u, i));
        }
        entries.Sort((a, b) =>
        {
            int ti = a.turnIndex.CompareTo(b.turnIndex);
            if (ti != 0) return ti;
            return b.unit.GetSpeed().CompareTo(a.unit.GetSpeed());
        });
        foreach (var e in entries)
            initiativeQueue.Add(e.unit);
        queueIndex = 0;
    }

    private void AdvanceToNextTurn()
    {
        if (combatEnded) return;
        CheckCombatEnd();
        if (combatEnded) return;
        OnTurnChanged?.Invoke();

        while (queueIndex < initiativeQueue.Count)
        {
            Unit next = initiativeQueue[queueIndex];
            queueIndex++;
            if (next == null || !next.gameObject.activeInHierarchy)
                continue;

            activeUnit = next;
            activeUnit.StartTurn();
            OnActiveUnitChanged?.Invoke(activeUnit);

            if (activeUnit.IsEnemy)
            {
                StartCoroutine(EnemyTurnRoutine());
                return;
            }

            if (!activeUnit.IsEnemy)
            {
                UnitActionSystem.Instance?.SetSelectedUnit(activeUnit);
                return;
            }
        }

        activeUnit = null;
        queueIndex = 0;
        roundNumber++;
        BuildInitiativeQueue();
        OnRoundStarted?.Invoke();
        if (initiativeQueue.Count > 0)
            AdvanceToNextTurn();
    }

    private void CheckCombatEnd()
    {
        Unit[] all = FindObjectsOfType<Unit>();
        int players = 0, enemies = 0;
        foreach (Unit u in all)
        {
            if (u == null || !u.gameObject.activeInHierarchy) continue;
            if (u.IsEnemy) enemies++; else players++;
        }
        if (players == 0) { combatEnded = true; OnCombatEnded?.Invoke(false); return; }
        if (enemies == 0) { combatEnded = true; OnCombatEnded?.Invoke(true); return; }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        AIBrain brain = activeUnit != null ? activeUnit.GetComponent<AIBrain>() : null;
        if (brain != null)
            yield return brain.ExecuteTurnCoroutine();
        else
            yield return new WaitForSeconds(0.5f);
        RequestEndTurn();
    }
}
