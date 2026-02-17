using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Wraithbound/AbilityData")]
public class AbilityData_SO : ScriptableObject
{
    public string abilityName;
    public int manaCost;
    public int range;
    public int damage;
    public TargetingType targetingType;
    public int aoeRadius;
    public List<StatusEffectType> appliedEffects = new List<StatusEffectType>();
}
