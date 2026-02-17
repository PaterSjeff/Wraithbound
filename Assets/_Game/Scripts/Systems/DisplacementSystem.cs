using UnityEngine;

public static class DisplacementSystem
{
    public struct PushResult
    {
        public bool success;
        public GridPosition finalPosition;
        public bool hitWall;
        public bool hitUnit;
        public IDamageable collisionTarget;
    }

    /// <summary>
    /// Push a unit in the direction away from pusherPos, up to distance tiles. Updates grid and unit position.
    /// </summary>
    public static PushResult Push(Unit unit, GridPosition pusherPos, int distance, bool damageOnCollision = true)
    {
        GridPosition from = unit.GetGridPosition();
        int dx = Mathf.Clamp(from.x - pusherPos.x, -1, 1);
        int dz = Mathf.Clamp(from.z - pusherPos.z, -1, 1);
        if (distance <= 0 || (dx == 0 && dz == 0)) return new PushResult { success = false, finalPosition = from };

        PushResult result = Push(from, new GridPosition(from.x + dx, from.z + dz), distance, damageOnCollision);
        if (result.success && result.finalPosition != from)
            unit.SetGridPosition(result.finalPosition);
        return result;
    }

    /// <summary>
    /// Push a pushable static object. Updates grid and object position.
    /// </summary>
    public static PushResult Push(StaticObject staticObj, GridPosition pusherPos, int distance, bool damageOnCollision = true)
    {
        if (staticObj == null || !staticObj.IsPushable) return new PushResult { success = false, finalPosition = staticObj.GridPosition };
        GridPosition from = staticObj.GridPosition;
        int dx = Mathf.Clamp(from.x - pusherPos.x, -1, 1);
        int dz = Mathf.Clamp(from.z - pusherPos.z, -1, 1);
        if (distance <= 0 || (dx == 0 && dz == 0)) return new PushResult { success = false, finalPosition = from };

        PushResult result = Push(from, new GridPosition(from.x + dx, from.z + dz), distance, damageOnCollision);
        if (result.success && result.finalPosition != from)
            staticObj.SetGridPosition(result.finalPosition);
        return result;
    }

    /// <summary>
    /// Resolve push from sourcePos toward targetPos (direction), up to distance tiles.
    /// Returns final position; does not move any object.
    /// </summary>
    public static PushResult Push(GridPosition sourcePos, GridPosition targetPos, int distance, bool damageOnCollision = true)
    {
        var result = new PushResult { success = false, finalPosition = sourcePos };
        if (distance <= 0) return result;

        GridSystem grid = GridSystem.Instance;
        if (!grid.IsValidGridPosition(sourcePos) || !grid.IsValidGridPosition(targetPos)) return result;

        int dx = Mathf.Clamp(targetPos.x - sourcePos.x, -1, 1);
        int dz = Mathf.Clamp(targetPos.z - sourcePos.z, -1, 1);
        if (dx == 0 && dz == 0) return result;

        GridPosition current = sourcePos;
        for (int i = 0; i < distance; i++)
        {
            GridPosition next = new GridPosition(current.x + dx, current.z + dz);
            if (!grid.IsValidGridPosition(next))
            {
                result.finalPosition = current;
                result.success = true;
                result.hitWall = true;
                return result;
            }

            GridObject cell = grid.GetGridObject(next);
            if (!cell.IsWalkable() && cell.GetUnit() == null)
            {
                result.finalPosition = current;
                result.success = true;
                result.hitWall = true;
                return result;
            }

            Unit unitThere = cell.GetUnit();
            if (unitThere != null)
            {
                result.finalPosition = current;
                result.success = true;
                result.hitUnit = true;
                result.collisionTarget = unitThere;
                if (damageOnCollision)
                    unitThere.TakeDamage(1);
                return result;
            }

            current = next;
        }

        result.finalPosition = current;
        result.success = true;
        return result;
    }

    /// <summary>
    /// Throw: move target to destination tile. Grid and transform updated immediately.
    /// </summary>
    public static void Throw(Unit unit, GridPosition destination)
    {
        if (unit == null || !GridSystem.Instance.IsValidGridPosition(destination)) return;
        GridObject cell = GridSystem.Instance.GetGridObject(destination);
        if (!cell.IsWalkable() || cell.GetUnit() != null) return;
        unit.SetGridPosition(destination);
    }

    public static void Throw(StaticObject staticObj, GridPosition destination)
    {
        if (staticObj == null || !staticObj.IsPushable || !GridSystem.Instance.IsValidGridPosition(destination)) return;
        GridObject cell = GridSystem.Instance.GetGridObject(destination);
        if (!cell.IsWalkable() || cell.GetUnit() != null || cell.GetStaticObject() != null) return;
        staticObj.SetGridPosition(destination);
    }
}
