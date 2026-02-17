using System.Collections.Generic;
using UnityEngine;

public static class TargetingHelper
{
    public static List<GridPosition> GetAffectedPositions(
        GridPosition origin,
        GridPosition targetPosition,
        TargetingType targetingType,
        int range,
        int aoeRadius)
    {
        var result = new List<GridPosition>();
        if (!GridSystem.Instance.IsValidGridPosition(origin)) return result;

        switch (targetingType)
        {
            case TargetingType.Self:
                result.Add(origin);
                break;

            case TargetingType.SingleTarget:
                if (GridSystem.Instance.IsValidGridPosition(targetPosition) && origin.Distance(targetPosition) <= range)
                    result.Add(targetPosition);
                break;

            case TargetingType.AoE:
                for (int x = targetPosition.x - aoeRadius; x <= targetPosition.x + aoeRadius; x++)
                {
                    for (int z = targetPosition.z - aoeRadius; z <= targetPosition.z + aoeRadius; z++)
                    {
                        var p = new GridPosition(x, z);
                        if (!GridSystem.Instance.IsValidGridPosition(p)) continue;
                        if (targetPosition.Distance(p) <= aoeRadius)
                            result.Add(p);
                    }
                }
                break;

            case TargetingType.Line:
                int dx = Mathf.Clamp(targetPosition.x - origin.x, -1, 1);
                int dz = Mathf.Clamp(targetPosition.z - origin.z, -1, 1);
                for (int i = 0; i <= range; i++)
                {
                    var p = new GridPosition(origin.x + dx * i, origin.z + dz * i);
                    if (!GridSystem.Instance.IsValidGridPosition(p)) break;
                    result.Add(p);
                }
                break;

            case TargetingType.Cone:
                dx = Mathf.Clamp(targetPosition.x - origin.x, -1, 1);
                dz = Mathf.Clamp(targetPosition.z - origin.z, -1, 1);
                for (int i = 1; i <= range; i++)
                {
                    for (int j = -i; j <= i; j++)
                    {
                        int cx = origin.x + dx * i + (dz != 0 ? j : 0);
                        int cz = origin.z + dz * i + (dx != 0 ? j : 0);
                        var p = new GridPosition(cx, cz);
                        if (GridSystem.Instance.IsValidGridPosition(p))
                            result.Add(p);
                    }
                }
                break;
        }

        return result;
    }

    public static List<GridPosition> GetValidTargetPositions(
        GridPosition origin,
        TargetingType targetingType,
        int range,
        int minRange,
        int aoeRadius)
    {
        var result = new List<GridPosition>();
        if (!GridSystem.Instance.IsValidGridPosition(origin)) return result;

        switch (targetingType)
        {
            case TargetingType.Self:
                result.Add(origin);
                break;

            case TargetingType.SingleTarget:
                for (int x = origin.x - range; x <= origin.x + range; x++)
                {
                    for (int z = origin.z - range; z <= origin.z + range; z++)
                    {
                        var p = new GridPosition(x, z);
                        if (!GridSystem.Instance.IsValidGridPosition(p)) continue;
                        int distance = origin.Distance(p);
                        if (distance >= minRange && distance <= range)
                            result.Add(p);
                    }
                }
                break;

            case TargetingType.AoE:
            case TargetingType.Line:
            case TargetingType.Cone:
                for (int x = origin.x - range - aoeRadius; x <= origin.x + range + aoeRadius; x++)
                {
                    for (int z = origin.z - range - aoeRadius; z <= origin.z + range + aoeRadius; z++)
                    {
                        var p = new GridPosition(x, z);
                        if (!GridSystem.Instance.IsValidGridPosition(p)) continue;
                        int distance = origin.Distance(p);
                        if (distance >= minRange && distance <= range + aoeRadius)
                            result.Add(p);
                    }
                }
                break;
        }

        return result;
    }
}
