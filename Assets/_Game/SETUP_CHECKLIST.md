# Wraithbound – Setup Checklist

Use this after opening the project in Unity. Your project already has layers **MousePlane**, **Obstacle**, and **Unit** in Tag Manager.

---

## 1. Scene hierarchy and managers

Create (or use) a **GameManager** GameObject and assign these components and references:

| Component        | Assign these in the Inspector |
|-----------------|---------------------------------|
| **GameManager**  | Input Manager, Grid System, Unit Action System, **Turn Manager**, **Pathfinding** |

- **TurnManager** and **Pathfinding** can be on the same GameObject as GameManager or on child objects; they just need to exist in the scene and be dragged into GameManager’s slots.
- **InputManager**, **GridSystem**, **UnitActionSystem** should also be in the scene and assigned (as before).

---

## 2. Grid System

On the **GridSystem** component:

- **Width** / **Height**: e.g. 10×10 (default).
- **Grid Debug Object Prefab**: Assign `_Game/Prefabs/GridDebugObject`.

No obstacle layer or raycasts are used anymore; obstacles are **StaticObject** instances (see below).

---

## 3. Ground / mouse plane (for clicking the grid)

You need something for the mouse raycast to hit so grid position works:

- A flat object (Quad or Plane) for the **ground**.
- Set its **Layer** to **MousePlane** (layer index 6).
- In **MouseWorld**, set **Mouse Plane Layer Mask** to only include **MousePlane**.

---

## 4. Units

### Player units

- Use the **Unit** prefab (`_Game/Prefabs/Unit.prefab`) or place it in the scene.
- Set **Is Enemy** = false.
- Set the unit’s **Layer** to **Unit** (index 8).
- Ensure it has:
  - **Unit** (script)
  - **Move Action**
  - **Attack Action**
- Optional: assign a **Unit Data** ScriptableObject (Create → Wraithbound → Unit Data). If unset, the Unit uses its inspector stats (Speed, Moves Per Turn, etc.).
- Position the unit so it sits on the grid; `GameManager` will call `Unit.Init()` and snap it to the grid.

### Enemy units

- Duplicate the Unit prefab or add another Unit in the scene.
- Set **Is Enemy** = true.
- Set **Layer** to **Unit**.
- Add the **AIBrain** component:
  - Assign an **AITemplate** (Create → Wraithbound → AITemplate), or leave empty and set **Unit Data** on the Unit with an **AI Template** reference; AIBrain will use that.
- Same as player: Move Action, Attack Action; optional **Unit Data**.

### UnitActionSystem

- **Unit Layer Mask**: set to the **Unit** layer so clicks register on units.

---

## 5. Optional: Unit Data and AI templates

- **Unit Data**: Create → Wraithbound → Unit Data. Set name, stats (HP, mana, speed, move/attack range, etc.), and optionally an **AI Template** for enemies. Assign to the Unit’s **Unit Data** field.
- **AI Template**: Create → Wraithbound → AITemplate. Set **Target Priority** (e.g. Closest, Highest Damage, Lowest HP). Assign to enemy Unit Data or to AIBrain’s **Template**).
- **Abilities**: Create → Wraithbound → Ability Data. Add an **AbilityAction** component to a unit and assign the ability; or add abilities to **Unit Data** and add **AbilityAction** components that use that data (you can wire one AbilityAction per ability).

---

## 6. Obstacles (static objects)

Obstacles are no longer done with layers/raycasts; they are **StaticObject**s on the grid.

- Create a **StaticObjectData** asset: Create → Wraithbound → StaticObject Data (HP, Is Pushable, etc.).
- Create a prefab with a **StaticObject** component and assign that **StaticObjectData**.
- Place instances in the scene on the grid. **GameManager** will call `Init()` on every **StaticObject** in the scene after the grid is created, so they register automatically. For runtime-spawned obstacles, call `StaticObject.Init(gridPosition)` after spawning.

---

## 7. UI

### Action buttons (Move, Attack, abilities)

- **ActionSelectionUI** needs:
  - **Action Button Container**: a parent Transform (e.g. empty GameObject or Layout Group) where buttons will be created.
  - **Action Button Prefab**: assign `_Game/Prefabs/UI/ActionButton_Prefab`.
- **ActionButton_Prefab** must have **ActionButtonUI** with:
  - **Text Mesh Pro**: label for the action name.
  - **Button**: the Button component.
  - **Selected Visual**: optional GameObject to show the selected action.

### Turn / round

- **TurnManagerUI** needs:
  - **End Turn Button**: the “End Turn” button.
  - **Turn Text**: TextMeshProUGUI for “Round X — Your Turn” / “Enemy Turn”.
  - **Initiative Order Text** (optional): another TextMeshProUGUI to show the initiative order; leave empty if you don’t want it.

### Busy overlay

- **ActionBusyUI**: assign **Busy Screen** (e.g. a full-screen panel that shows while an action is running).

---

## 8. Camera

- **Camera.main** must exist (tag the camera as **MainCamera**) so **UnitActionSystem** and **MouseWorld** raycasts work.

---

## 9. Quick test

1. One **player Unit** (Is Enemy = false, Layer = Unit) on the grid with Move + Attack.
2. One **enemy Unit** (Is Enemy = true, Layer = Unit) with **AIBrain** and an **AITemplate** (or Unit Data with AI Template).
3. Ground on **MousePlane**; **MouseWorld** mask = MousePlane.
4. **UnitActionSystem** Unit Layer Mask = Unit.
5. **GameManager** has all five references (Input, Grid, UnitActionSystem, TurnManager, Pathfinding).
6. **GridSystem** has **Grid Debug Object Prefab** assigned.
7. Play: your unit should get a turn, you can move/attack, then End Turn and the enemy should take a turn (AI moves or attacks).

---
