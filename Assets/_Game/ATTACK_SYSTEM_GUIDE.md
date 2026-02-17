# Attack System Guide

## Overview
The new attack system uses `AttackData_SO` ScriptableObjects to define flexible, configurable attacks. This allows you to create various attack types without writing code.

## Creating a New Attack

1. **Right-click in Project** → `Create` → `Tactical Combat` → `Attack Data`
2. **Name it** (e.g., "MeleeSlash", "Fireball", "Cleave")
3. **Configure the properties** in the Inspector

## Attack Properties

### Basic Info
- **Attack Name**: Display name (shown in UI)
- **Icon**: Visual icon for the attack (optional)

### Targeting
- **Targeting Type**: How the attack targets
  - `SingleTarget`: One enemy at a time
  - `AoE`: Area of effect around a target point
  - `Line`: Straight line from caster
  - `Cone`: Cone-shaped area
  - `Self`: Targets the caster
- **Range**: Maximum distance to target
- **AOE Radius**: For AOE attacks, how many tiles from the center point

### Damage
- **Base Damage**: Fixed damage amount
- **Use Unit Attack Damage**: If true, uses the unit's attackDamage stat instead of baseDamage

### Resource Cost
- **Costs Attack Action**: If true, uses one attack per turn
- **Mana Cost**: If not using attack action, how much mana it costs

### Requirements
- **Min Range**: Minimum distance (for ranged attacks that can't be used point-blank)
- **Requires Line of Sight**: Whether obstacles block the attack (future feature)

### Animation
- **Is Melee Animation**: If true, unit lunges toward target
- **Animation Duration**: How long the attack animation takes
- **Lunge Distance**: For melee, how far forward the unit moves

### Effects
- **Status Effects**: Status effects to apply on hit (future feature)
- **Status Effect Duration**: How long status effects last

## Example Attacks

### Basic Melee Attack
```
Attack Name: "Slash"
Targeting Type: SingleTarget
Range: 1
Base Damage: 1
Use Unit Attack Damage: ✓
Costs Attack Action: ✓
Is Melee Animation: ✓
Lunge Distance: 0.5
```

### Ranged Bow Attack
```
Attack Name: "Shoot Arrow"
Targeting Type: SingleTarget
Range: 5
Min Range: 2
Use Unit Attack Damage: ✓
Costs Attack Action: ✓
Is Melee Animation: ✗
Animation Duration: 0.3
```

### Melee AOE Cleave
```
Attack Name: "Cleave"
Targeting Type: AoE
Range: 1
AOE Radius: 1
Use Unit Attack Damage: ✓
Costs Attack Action: ✓
Is Melee Animation: ✓
Lunge Distance: 0.3
```

### Ranged AOE Fireball
```
Attack Name: "Fireball"
Targeting Type: AoE
Range: 6
AOE Radius: 2
Base Damage: 3
Use Unit Attack Damage: ✗
Costs Attack Action: ✗
Mana Cost: 3
Is Melee Animation: ✗
Animation Duration: 0.5
```

### Cone Breath Attack
```
Attack Name: "Flame Breath"
Targeting Type: Cone
Range: 3
AOE Radius: 2 (cone width)
Base Damage: 2
Costs Attack Action: ✓
Is Melee Animation: ✗
```

## Assigning Attacks to Units

### Option 1: Via UnitData_SO (Recommended)
1. Open or create a `UnitData_SO` asset
2. Find the `Actions` section
3. Add attacks to the `Attacks` list
4. The unit will automatically get these attacks when spawned

### Option 2: Manually on Prefab
1. Open the Unit prefab
2. Add a `ConfigurableAttackAction` component
3. Drag an `AttackData_SO` into the `Attack Data` field
4. Repeat for each attack

## Differences from Abilities

| Feature | Attacks | Abilities |
|---------|---------|-----------|
| Resource | Attack actions OR mana | Always mana |
| Damage | Typically damage-focused | Can be utility, healing, etc. |
| AI Usage | High priority | Medium priority |
| Examples | Slash, Shoot, Cleave | Heal, Buff, Teleport |

## Migration from Old System

The old `AttackAction` still works for backward compatibility:
- Units without `UnitData_SO` or with empty attacks list will get a default `AttackAction`
- This ensures existing units continue to work

## Notes

- Units can have **multiple attacks** (e.g., sword slash + shield bash)
- Attacks can cost mana instead of attack actions (for special attacks)
- The AI will automatically use all configured attacks
- Attacks that cost attack actions have higher AI priority than abilities
