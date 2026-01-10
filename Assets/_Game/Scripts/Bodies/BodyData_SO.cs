using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBody", menuName = "Wraithbound/BodyData")]
public class BodyData_SO : ScriptableObject
{
    [Header("Visuals")]
    public GameObject prefab;
    
    [Header("Stats")]
    public int maxStructure = 3; // Standard HP for most units
    
    // The core mechanic: [Minor, Major, Fatal]
    // Example: [10, 15, 20]
    public int[] defenseThresholds = new int[] { 10, 15, 20 }; 

    [Header("Weaknesses")]
    public List<string> weaknessTags; // Simple string list for prototype
}