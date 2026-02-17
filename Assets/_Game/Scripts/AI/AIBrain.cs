using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    [SerializeField] private AITemplate_SO template;
    [SerializeField] private float actionDelaySeconds = 0.5f;

    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        if (template == null && unit?.UnitData != null)
            template = unit.UnitData.aiTemplate;
    }

    public void SetTemplate(AITemplate_SO t) => template = t ?? template;

    public IEnumerator ExecuteTurnCoroutine()
    {
        if (unit == null || !unit.gameObject.activeInHierarchy) yield break;

        if (template == null)
        {
            yield return new WaitForSeconds(actionDelaySeconds);
            yield break;
        }

        // Keep taking actions until no more resources available
        int maxActionsPerTurn = 10; // Safety limit to prevent infinite loops
        int actionsTaken = 0;

        while (actionsTaken < maxActionsPerTurn)
        {
            // Check if unit has any resources left
            if (!unit.CanMove() && !unit.CanAttack() && unit.GetCurrentMana() <= 0)
                break;

            Unit[] allUnits = FindObjectsOfType<Unit>();
            Unit target = AITargetEvaluator.GetBestTarget(unit, template, allUnits);

            if (target == null)
                break;

            var choice = AIActionEvaluator.GetBestAction(unit, target, template);

            // No valid action found
            if (choice.action == null || choice.score <= int.MinValue)
                break;

            // Can't afford this action
            if (!unit.CanSpendActionPointsToTakeAction(choice.action))
                break;

            yield return new WaitForSeconds(actionDelaySeconds);

            // Execute the action
            unit.SpendActionPoints(choice.action);
            bool done = false;
            choice.action.TakeAction(choice.targetPosition, () => done = true);
            while (!done)
                yield return null;

            actionsTaken++;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
