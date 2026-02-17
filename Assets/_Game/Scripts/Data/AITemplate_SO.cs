using UnityEngine;

[CreateAssetMenu(fileName = "NewAITemplate", menuName = "Wraithbound/AITemplate")]
public class AITemplate_SO : ScriptableObject
{
    public string templateName;
    public AITargetPriority targetPriority;
    [Range(0f, 1f)] public float aggressionLevel = 0.5f;
    public bool prefersRangedPosition;
    public bool retreatsWhenLow;
    [Range(0f, 1f)] public float retreatHealthThreshold = 0.25f;
}

public enum AITargetPriority
{
    Closest,
    HighestDamage,
    LowestHP,
    StatusCondition,
    Random
}
