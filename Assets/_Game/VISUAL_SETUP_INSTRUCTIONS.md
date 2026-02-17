# Visual Setup Instructions

## Tile Highlighting & DOTween Animations Setup

### 1. TileHighlightManager Setup
1. In the Hierarchy, create a new empty GameObject and name it `TileHighlightManager`
2. Add the `TileHighlightManager` component to it
3. Configure the colors in the Inspector:
   - **Valid Target Highlight Colors** (shows all valid targets):
     - **Move Highlight Color**: Light blue (default: R:0.3, G:0.6, B:1, A:0.5)
     - **Attack Highlight Color**: Light red (default: R:1, G:0.3, B:0.3, A:0.5)
     - **Ability Highlight Color**: Light purple (default: R:0.8, G:0.3, B:1, A:0.5)
   - **Hover Preview Colors** (shows what will happen when you click):
     - **Hover Move Color**: Brighter blue (default: R:0.5, G:0.8, B:1, A:0.7)
     - **Hover Attack Color**: Brighter red (default: R:1, G:0.5, B:0.5, A:0.7)
     - **Hover Ability Color**: Brighter purple (default: R:1, G:0.5, B:1, A:0.7)
4. **IMPORTANT**: Drag the `TileHighlightManager` GameObject to the GameManager's Inspector:
   - Select `GameManager` in the Hierarchy
   - Drag `TileHighlightManager` to the `Tile Highlight Manager` field

### 2. GridDebugObject Prefab Update
1. Open the `GridDebugObject` prefab (located in `Assets/_Game/Prefabs/`)
2. **Add Highlight child**:
   - Add a child GameObject and name it `Highlight`
   - Add a `Quad` mesh to this child (or create a plane)
   - Position it at Y = 0.01 (slightly above the ground to avoid z-fighting)
   - Scale it to match your cell size (e.g., X:0.9, Y:1, Z:0.9 for a 1x1 cell)
   - Add a `MeshRenderer` component if not already present
   - Create a material named `TileHighlight` with transparency enabled
   - Assign the `Highlight` child's MeshRenderer to the `highlightRenderer` field in `GridDebugObject`
3. **Add Hover Preview child**:
   - Duplicate the `Highlight` child and rename it to `HoverPreview`
   - Position it at Y = 0.02 (above the highlight layer)
   - Scale slightly larger (e.g., X:0.95, Y:1, Z:0.95)
   - Use the same material or create a similar one
   - Assign the `HoverPreview` child's MeshRenderer to the `hoverRenderer` field in `GridDebugObject`

### 3. Unit Prefab Setup for Damage Animation
1. Open each Unit prefab
2. Ensure the unit has a `MeshRenderer` component (should already exist)
3. The `Unit` script will automatically find the MeshRenderer (checks children too)
4. **Optional**: Manually assign the `Unit Mesh Renderer` field in Inspector if auto-detection fails
5. **Tune damage flash parameters** in the Inspector:
   - **Damage Flash Color**: Color to flash when hit (default: Red)
   - **Damage Flash Duration**: How long each flash phase lasts (default: 0.15s)
   - **Damage Punch Scale**: How much to scale on hit (default: 0.1)
   - **Damage Punch Duration**: Duration of the punch effect (default: 0.25s)
6. **IMPORTANT**: The material must support color changes
   - For URP: Use shaders like `Universal Render Pipeline/Lit` or `Universal Render Pipeline/Unlit`
   - Avoid shaders that don't have a `_BaseColor` or `_Color` property

### 4. DOTween Setup
DOTween is already added to the project. If you see any import/setup dialogs:
1. Go to `Tools > Demigiant > DOTween Utility Panel`
2. Click `Setup DOTween` and select appropriate modules
3. Recommended: Enable `TextMeshPro` support if using TMP

### 5. Testing
1. Enter Play Mode
2. Select a unit
3. Click the "Move" action button
   - Valid move tiles should highlight in **blue** (faint)
   - **Hover over tiles** - the hovered tile should show **brighter blue**
4. Click the "Attack" action button
   - Valid attack tiles should highlight in **red** (faint)
   - **Hover over valid targets** - the target should show **brighter red**
5. Test AOE abilities/attacks
   - Valid target points highlight in faint color
   - **Hover over a target point** - all affected tiles show **bright preview**
6. Execute a move
   - Unit should **hop** along the path
7. Execute an attack
   - Attacker should **lunge** forward and back
   - Target should **flash red** and **punch scale**

### 6. Tuning Parameters
You can adjust animation parameters on each action:

**MoveAction** (on Unit prefab):
- `Hop Height`: Height of the hop (default: 0.3)
- `Hop Duration`: Duration of each hop (default: 0.3s)

**AttackAction** (on Unit prefab):
- `Attack Duration`: Total attack animation time (default: 0.5s)
- `Lunge Distance`: How far forward the attacker moves (default: 0.5)

### 7. Troubleshooting
- **Highlights not showing**: Check that the `Highlight` child is active and has a MeshRenderer assigned
- **Highlights wrong color**: Verify the TileHighlightManager colors in Inspector
- **Animations not playing**: Check Console for DOTween errors, ensure DOTween is set up
- **Damage flash not working**: 
  - Check Console for `[Unit] Playing damage flash` message - if you see it, the tween is running
  - If message shows but no visual change: Your material's shader may not support color animation
  - Try using `Universal Render Pipeline/Lit` shader on your unit's material
  - Make sure the material has a color property (not just a texture)
  - Check that `Unit Mesh Renderer` field is assigned (or auto-detected)
  - Increase `Damage Flash Duration` to make it more visible (try 0.3s)
