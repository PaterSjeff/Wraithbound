using System.Collections.Generic;
using UnityEngine;

public class AIActionEvaluator
{
    public struct ActionChoice
    {
        public BaseAction action;
        public GridPosition targetPosition;
        public int score;
    }

    public static ActionChoice GetBestAction(Unit aiUnit, Unit targetUnit, AITemplate_SO template)
    {
        var choice = new ActionChoice { score = int.MinValue };
        if (aiUnit == null || targetUnit == null) return choice;

        GridPosition myPos = aiUnit.GetGridPosition();
        GridPosition targetPos = targetUnit.GetGridPosition();
        int distance = myPos.Distance(targetPos);

        // Try all attack actions (both legacy AttackAction and ConfigurableAttackAction)
        BaseAction[] allActions = aiUnit.GetActions();
        foreach (BaseAction action in allActions)
        {
            if (action.GetActionResourceType() != ActionResourceType.Attack) continue;
            if (!action.CanExecute()) continue;

            var validTargets = action.GetValidActionGridPositionList();
            if (validTargets.Contains(targetPos))
            {
                choice.action = action;
                choice.targetPosition = targetPos;
                choice.score = 100 + targetUnit.CurrentHP;
                return choice;
            }
        }

        MoveAction move = aiUnit.GetAction<MoveAction>();
        if (move != null && aiUnit.CanMove())
        {
            var validMoves = move.GetValidActionGridPositionList();
            GridPosition bestMove = myPos;
            int bestDist = distance;

            // Find the move that gets closest to the target
            foreach (GridPosition pos in validMoves)
            {
                int d = pos.Distance(targetPos);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestMove = pos;
                }
            }

            // Move if we found a position closer than current
            if (bestMove != myPos)
            {
                choice.action = move;
                choice.targetPosition = bestMove;
                // Higher score if we'll be in attack range after moving
                if (bestDist <= aiUnit.GetAttackRange())
                    choice.score = 70 - bestDist; // Good move, sets up for attack
                else
                    choice.score = 40 - bestDist; // Okay move, getting closer
                return choice;
            }
        }

        var abilities = aiUnit.GetComponents<AbilityAction>();
        foreach (AbilityAction ability in abilities)
        {
            if (ability == null || !ability.CanExecute()) continue;
            var valid = ability.GetValidActionGridPositionList();
            if (valid.Contains(targetPos))
            {
                choice.action = ability;
                choice.targetPosition = targetPos;
                choice.score = 80;
                return choice;
            }
        }

        return choice;
    }
}
