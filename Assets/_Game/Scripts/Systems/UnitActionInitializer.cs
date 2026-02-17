using UnityEngine;

/// <summary>
/// Helper script to dynamically add action components to units based on their UnitData_SO
/// Call this after Unit.Init() or during unit spawning
/// </summary>
public class UnitActionInitializer : MonoBehaviour
{
    public static void InitializeActions(Unit unit)
    {
        if (unit == null || unit.UnitData == null) return;

        // Add MoveAction (always present)
        if (unit.GetAction<MoveAction>() == null)
            unit.gameObject.AddComponent<MoveAction>();

        // Add AttackAction (legacy fallback if no attacks defined)
        if (unit.UnitData.attacks.Count == 0)
        {
            if (unit.GetAction<AttackAction>() == null)
                unit.gameObject.AddComponent<AttackAction>();
        }
        else
        {
            // Add ConfigurableAttackAction for each attack in UnitData
            foreach (AttackData_SO attackData in unit.UnitData.attacks)
            {
                if (attackData == null) continue;
                
                ConfigurableAttackAction attackAction = unit.gameObject.AddComponent<ConfigurableAttackAction>();
                attackAction.SetAttackData(attackData);
            }
        }

        // Add AbilityAction for each ability in UnitData
        foreach (AbilityData_SO abilityData in unit.UnitData.abilities)
        {
            if (abilityData == null) continue;
            
            AbilityAction abilityAction = unit.gameObject.AddComponent<AbilityAction>();
            abilityAction.SetAbilityData(abilityData);
        }
    }
}
