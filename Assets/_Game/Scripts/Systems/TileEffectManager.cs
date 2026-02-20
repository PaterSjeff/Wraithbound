using UnityEngine;

/// <summary>
/// Manages tile effects like damage, status effects, and terrain bonuses
/// </summary>
public class TileEffectManager : MonoBehaviour
{
    public static TileEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called when a unit enters a tile (after movement)
    /// </summary>
    public void ApplyTileEffectsOnEnter(Unit unit, GridPosition position)
    {
        if (unit == null || GridSystem.Instance == null) return;

        GridObject gridObj = GridSystem.Instance.GetGridObject(position);
        if (gridObj == null) return;

        TileType_SO tileType = gridObj.GetTileType();
        if (tileType == null) return;

        // Apply damage
        if (tileType.damageOnEnter > 0)
        {
            unit.TakeDamage(tileType.damageOnEnter);
            Debug.Log($"{unit.name} took {tileType.damageOnEnter} damage from {tileType.tileName}");
        }

        // Apply status effects
        if (tileType.effectsOnEnter != null && tileType.effectsOnEnter.Length > 0)
        {
            // TODO: Apply status effects when status system is implemented
            foreach (StatusEffectType effect in tileType.effectsOnEnter)
            {
                Debug.Log($"{unit.name} got {effect} from entering {tileType.tileName}");
            }
        }
    }

    /// <summary>
    /// Called at the start of a unit's turn while standing on a tile
    /// </summary>
    public void ApplyTileEffectsPerTurn(Unit unit)
    {
        if (unit == null || GridSystem.Instance == null) return;

        GridPosition position = unit.GetGridPosition();
        GridObject gridObj = GridSystem.Instance.GetGridObject(position);
        if (gridObj == null) return;

        TileType_SO tileType = gridObj.GetTileType();
        if (tileType == null) return;

        // Apply per-turn damage
        if (tileType.damagePerTurn > 0)
        {
            unit.TakeDamage(tileType.damagePerTurn);
            Debug.Log($"{unit.name} took {tileType.damagePerTurn} damage per turn from {tileType.tileName}");
        }

        // Apply ongoing status effects
        if (tileType.effectsWhileStanding != null && tileType.effectsWhileStanding.Length > 0)
        {
            // TODO: Apply status effects when status system is implemented
            foreach (StatusEffectType effect in tileType.effectsWhileStanding)
            {
                Debug.Log($"{unit.name} is affected by {effect} while on {tileType.tileName}");
            }
        }
    }

    /// <summary>
    /// Get combat modifiers from the tile a unit is standing on
    /// </summary>
    public TileCombatModifiers GetTileModifiers(GridPosition position)
    {
        TileCombatModifiers modifiers = new TileCombatModifiers();
        
        if (GridSystem.Instance == null) return modifiers;

        GridObject gridObj = GridSystem.Instance.GetGridObject(position);
        if (gridObj == null) return modifiers;

        TileType_SO tileType = gridObj.GetTileType();
        if (tileType == null) return modifiers;

        modifiers.dodgeBonus = tileType.dodgeBonus;
        modifiers.defenseBonus = tileType.defenseBonus;
        modifiers.attackBonus = tileType.attackBonus;
        modifiers.hasCover = tileType.providesCover;
        modifiers.coverDamageReduction = tileType.coverDamageReduction;

        return modifiers;
    }
}

public struct TileCombatModifiers
{
    public int dodgeBonus;
    public int defenseBonus;
    public int attackBonus;
    public bool hasCover;
    public int coverDamageReduction;
}
