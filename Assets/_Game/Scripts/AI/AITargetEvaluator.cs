using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AITargetEvaluator
{
    public static Unit GetBestTarget(Unit aiUnit, AITemplate_SO template, IEnumerable<Unit> allUnits)
    {
        if (template == null || aiUnit == null) return null;
        GridPosition myPos = aiUnit.GetGridPosition();
        var enemies = allUnits.Where(u => u != null && u.gameObject.activeInHierarchy && u.IsEnemy != aiUnit.IsEnemy).ToList();
        if (enemies.Count == 0) return null;

        Unit best = null;
        float bestScore = float.MinValue;

        foreach (Unit target in enemies)
        {
            float score = ScoreTarget(aiUnit, target, myPos, template.targetPriority);
            if (score > bestScore)
            {
                bestScore = score;
                best = target;
            }
        }

        return best;
    }

    private static float ScoreTarget(Unit aiUnit, Unit target, GridPosition myPos, AITargetPriority priority)
    {
        GridPosition targetPos = target.GetGridPosition();
        int distance = myPos.Distance(targetPos);

        switch (priority)
        {
            case AITargetPriority.Closest:
                return distance <= 0 ? float.MaxValue : 1000f / (distance + 1);
            case AITargetPriority.HighestDamage:
                return target.GetAttackDamage();
            case AITargetPriority.LowestHP:
                return target.CurrentHP <= 0 ? float.MinValue : 1000f / (target.CurrentHP + 1);
            case AITargetPriority.StatusCondition:
                return 1f;
            case AITargetPriority.Random:
                return Random.value;
            default:
                return 1f / (distance + 1);
        }
    }
}
