using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Wraithbound/UnitData")]
public class UnitData_SO : ScriptableObject
{
    public string unitName;
    public GameObject prefab;

    [Header("Stats")]
    public int maxHP = 10;
    public int maxMana = 5;
    public int speed = 10;
    public int moveRange = 4;
    public int attackDamage = 2;
    public int attackRange = 1;

    [Header("Turn Modifiers")]
    public int turnsPerRound = 1;
    public int attacksPerTurn = 1;
    public int movesPerTurn = 1;

    [Header("Actions")]
    public List<AttackData_SO> attacks = new List<AttackData_SO>();
    public List<AbilityData_SO> abilities = new List<AbilityData_SO>();

    [Header("AI (enemies only)")]
    public AITemplate_SO aiTemplate;
}
