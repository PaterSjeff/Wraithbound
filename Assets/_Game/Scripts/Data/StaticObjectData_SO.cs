using UnityEngine;

[CreateAssetMenu(fileName = "NewStaticObject", menuName = "Wraithbound/StaticObjectData")]
public class StaticObjectData_SO : ScriptableObject
{
    public string objectName;
    public GameObject prefab;
    public int maxHP;
    public bool isPushable;
    public bool blocksMovement = true;
    public bool blocksProjectiles = true;
}
