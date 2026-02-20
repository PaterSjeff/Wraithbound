# Tile Type System Guide

## Overview
The tile system allows you to create different terrain types with various effects on movement, combat, and unit status.

## Creating Tile Types

### 1. Create a TileType ScriptableObject
Right-click in Project → `Create` → `Tactical Combat` → `Tile Type`

### 2. Configure Properties

#### Movement
- **Movement Cost Multiplier**: How much this tile costs to move through
  - `1.0` = Normal terrain
  - `2.0` = Difficult terrain (costs 2 movement per tile)
  - `0.5` = Easy terrain (costs half movement)
- **Blocks Movement**: If true, units can't enter this tile at all

#### Status Effects on Enter
- **Effects On Enter**: Status effects applied when entering the tile
- **Effect Duration**: How long the effects last (in turns)
- **Damage On Enter**: Immediate damage when entering (e.g., spike trap)

#### Status Effects While Standing
- **Effects While Standing**: Effects applied each turn while on this tile
- **Damage Per Turn**: Damage dealt each turn (e.g., lava, poison gas)

#### Combat Modifiers
- **Dodge Bonus**: Increased dodge chance (0-100%)
- **Defense Bonus**: Extra defense points
- **Attack Bonus**: Extra attack damage
- **Provides Cover**: Reduces incoming ranged damage
- **Cover Damage Reduction**: Percentage of damage reduced (e.g., 50%)

#### Visual
- **Tile Color**: Tint color for the tile
- **Tile Sprite**: Custom sprite for the tile
- **Tile Effect**: Particle effect to spawn
- **Show Visual Indicator**: Whether to show visual effects

## Example Tile Types

### Normal Grass
```
Tile Name: "Grass"
Movement Cost: 1.0
All other values: default/zero
```

### Tall Grass (Stealth)
```
Tile Name: "Tall Grass"
Movement Cost: 1.5
Dodge Bonus: 20
Color: Dark Green
```

### Lava
```
Tile Name: "Lava"
Movement Cost: 2.0
Damage On Enter: 2
Damage Per Turn: 1
Effects While Standing: Burning
Color: Orange-Red
```

### Water/Mud
```
Tile Name: "Mud"
Movement Cost: 3.0
Dodge Bonus: -10 (harder to dodge in mud)
Color: Brown
```

### Rock Cover
```
Tile Name: "Rock"
Movement Cost: 1.0
Provides Cover: ✓
Cover Damage Reduction: 50
Defense Bonus: 2
Color: Gray
```

### Ice
```
Tile Name: "Ice"
Movement Cost: 0.5 (slide across it)
Dodge Bonus: -20 (slippery)
Color: Light Blue
```

### Spike Trap
```
Tile Name: "Spikes"
Damage On Enter: 3
Effects On Enter: Bleeding
Effect Duration: 2
Color: Dark Red
```

### Healing Spring
```
Tile Name: "Healing Spring"
Damage Per Turn: -2 (negative = healing)
Effects While Standing: Regeneration
Color: Light Green
```

## Assigning Tiles to the Grid

### Method 1: Programmatically (Recommended for generated levels)
```csharp
GridObject gridObj = GridSystem.Instance.GetGridObject(new GridPosition(x, z));
gridObj.SetTileType(lavaTileType);
```

### Method 2: In Editor (For designed levels)
You'll need to create a level editor tool or manually set tile types during grid initialization in GridSystem.cs

## How It Works

### Movement Costs
- Pathfinding automatically accounts for tile movement costs
- A tile with 2.0 cost will take 2 movement points to enter
- Units with 4 movement can only cross 2 tiles of difficult terrain (2.0 cost each)

### Tile Effects Application

**On Enter (SetGridPosition):**
- Damage is dealt immediately
- Status effects are applied with specified duration

**Each Turn (StartTurn):**
- Units take per-turn damage
- Ongoing status effects are applied

### Combat Modifiers

**Usage (future implementation):**
```csharp
TileCombatModifiers mods = TileEffectManager.Instance.GetTileModifiers(unit.GetGridPosition());
int finalDodge = basedodge + mods.dodgeBonus;
int finalDefense = baseDefense + mods.defenseBonus;
int finalAttack = baseAttack + mods.attackBonus;
```

## Integration Points

The system is already integrated:
- ✓ **Pathfinding**: Movement costs affect path calculation
- ✓ **Unit Movement**: Tile effects applied on enter
- ✓ **Turn System**: Per-turn effects applied at turn start
- ⏳ **Combat System**: Modifiers ready for implementation (when you add hit/dodge mechanics)
- ⏳ **Status System**: Effect application ready (waiting for status effect implementation)

## Visual Feedback (Future Enhancement)

You can add visual indicators:
1. Tint GridDebugObject colors based on tile type
2. Spawn particle effects (fire for lava, sparkles for healing)
3. Add tile-specific sprites/textures
4. Show movement cost numbers when hovering

## Notes

- Tile types are checked in this order: TileType → StaticObject → Base walkability
- Movement cost is multiplicative (tile cost × distance)
- Status effects are logged but need a status effect system to work fully
- Combat modifiers are accessible but need to be integrated into your hit/damage calculations
