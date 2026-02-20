using UnityEngine;

[CreateAssetMenu(fileName = "New TileType", menuName = "Tactical Combat/Tile Type")]
public class TileType_SO : ScriptableObject
{
    [Header("Basic Info")]
    public string tileName = "Normal";
    public Color tileColor = Color.white;
    public Sprite tileSprite;
    
    [Header("Movement")]
    [Tooltip("Movement cost multiplier. 1 = normal, 2 = costs double, 0.5 = costs half")]
    public float movementCostMultiplier = 1f;
    [Tooltip("Blocks movement entirely")]
    public bool blocksMovement = false;
    
    [Header("Status Effects on Enter")]
    [Tooltip("Status effects applied when a unit enters this tile")]
    public StatusEffectType[] effectsOnEnter;
    [Tooltip("Duration of status effects (in turns)")]
    public int effectDuration = 2;
    [Tooltip("Damage dealt when entering the tile")]
    public int damageOnEnter = 0;
    
    [Header("Status Effects While Standing")]
    [Tooltip("Status effects applied each turn while standing on this tile")]
    public StatusEffectType[] effectsWhileStanding;
    [Tooltip("Damage dealt each turn while on this tile")]
    public int damagePerTurn = 0;
    
    [Header("Combat Modifiers")]
    [Tooltip("Dodge chance bonus while on this tile (0-100)")]
    public int dodgeBonus = 0;
    [Tooltip("Defense bonus while on this tile")]
    public int defenseBonus = 0;
    [Tooltip("Attack bonus while on this tile")]
    public int attackBonus = 0;
    [Tooltip("Provides cover (reduces ranged damage)")]
    public bool providesCover = false;
    [Tooltip("Damage reduction from cover (percentage)")]
    public int coverDamageReduction = 50;
    
    [Header("Visual")]
    [Tooltip("Particle effect to spawn on this tile")]
    public GameObject tileEffect;
    [Tooltip("Show visual indicator (like grass, fire, etc.)")]
    public bool showVisualIndicator = true;
}
