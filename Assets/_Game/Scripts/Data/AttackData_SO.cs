using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Tactical Combat/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    [Header("Basic Info")]
    public string attackName = "Attack";
    public Sprite icon;
    
    [Header("Targeting")]
    public TargetingType targetingType = TargetingType.SingleTarget;
    public int range = 1;
    [Tooltip("For AOE attacks: radius from the target point")]
    public int aoeRadius = 0;
    
    [Header("Damage")]
    public int baseDamage = 1;
    [Tooltip("If true, uses unit's attack damage stat as base")]
    public bool useUnitAttackDamage = true;
    
    [Header("Resource Cost")]
    [Tooltip("If true, costs an attack action. If false, costs mana.")]
    public bool costsAttackAction = true;
    public int manaCost = 0;
    
    [Header("Requirements")]
    [Tooltip("Minimum range (for ranged attacks that can't target adjacent)")]
    public int minRange = 0;
    [Tooltip("Does this attack require line of sight?")]
    public bool requiresLineOfSight = false;
    
    [Header("Animation")]
    [Tooltip("If true, attacker moves toward target (melee lunge)")]
    public bool isMeleeAnimation = true;
    public float animationDuration = 0.5f;
    public float lungeDistance = 0.5f;
    
    [Header("Effects")]
    [Tooltip("Apply status effects on hit")]
    public StatusEffectType[] statusEffects;
    public int statusEffectDuration = 2;
}
