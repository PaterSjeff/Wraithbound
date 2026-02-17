# Initiative Order UI Setup Guide

## Overview
The Initiative Order UI shows the turn order on the left side of the screen, with the active unit at the top and a visual separator showing when the round ends.

## Required UI Structure

### 1. Create the Main Canvas Structure
In your UI Canvas hierarchy, create:

```
Canvas
└── InitiativeOrderPanel (left side of screen)
    ├── Background (Image - optional dark background)
    ├── Title (TextMeshPro - "Turn Order")
    └── ScrollView
        └── Viewport
            └── EntryContainer (Vertical Layout Group)
```

### 2. InitiativeOrderPanel Setup
- **Anchor**: Top-Left
- **Position**: X: 10, Y: -10 from top-left
- **Size**: Width: 200-250, Height: Auto or full screen height
- Add `InitiativeOrderUI` component to this GameObject

### 3. EntryContainer Setup (the container for all entries)
- Add `Vertical Layout Group` component:
  - **Child Alignment**: Upper Center
  - **Child Force Expand**: Width ✓, Height ✗
  - **Spacing**: 10
- Add `Content Size Fitter` component:
  - **Vertical Fit**: Preferred Size
- Drag this to the `Entry Container` field in `InitiativeOrderUI`

### 4. Create Unit Entry Prefab
Create a prefab named `UnitEntryPrefab`:

```
UnitEntry (GameObject)
├── Background (Image)
├── Content (Horizontal Layout Group)
│   ├── ActiveIndicator (Image - arrow or highlight)
│   ├── Icon (Image - unit portrait)
│   └── NameText (TextMeshProUGUI)
```

**UnitEntry Setup:**
- Add `Layout Element` component:
  - **Min Height**: 50-60
  - **Preferred Height**: 60
- Add `InitiativeEntryUI` component
- Assign all child references in the Inspector

**Background (Image):**
- Color will be set dynamically (player/enemy/active)

**ActiveIndicator (Image):**
- Small arrow or highlight icon
- Will be shown/hidden based on active state

**Icon (Image):**
- Square image for unit portrait
- Size: 40x40

**NameText (TextMeshProUGUI):**
- Font Size: 14-16
- Alignment: Middle Left

Drag this prefab to the `Unit Entry Prefab` field in `InitiativeOrderUI`

### 5. Create Round Separator Prefab
Create a prefab named `RoundSeparatorPrefab`:

```
RoundSeparator (GameObject)
├── Line (Image - horizontal line)
└── Text (TextMeshProUGUI - "Next Round")
```

**RoundSeparator Setup:**
- Add `Layout Element` component:
  - **Min Height**: 30-40
  - **Preferred Height**: 40
- Style with bright/contrasting colors (yellow/gold)

Drag this prefab to the `Round Separator Prefab` field in `InitiativeOrderUI`

## Visual Style Recommendations

### Colors
- **Player Units**: Blue tint (R:0.3, G:0.6, B:1, A:0.8)
- **Enemy Units**: Red tint (R:1, G:0.3, B:0.3, A:0.8)
- **Active Unit**: Bright yellow/gold (R:1, G:1, B:0.5, A:1)
- **Inactive Units**: Same as team color but with alpha: 0.6
- **Round Separator**: Gold/Yellow (R:1, G:0.9, B:0.3)

### Layout Tips
- Keep entry height consistent (60px recommended)
- Use spacing of 10px between entries
- Round separator should be visually distinct (different height/color)
- Active indicator should be obvious (arrow, glow, or highlight)

## How It Works

1. **Turn Start**: Active unit appears at the top with bright highlighting
2. **Scroll Through Turn**: As units take turns, the list updates with current unit always on top
3. **Round Separator**: Shows where the current round ends and next round begins
4. **Visual Feedback**: 
   - Current unit: Bright/highlighted
   - Upcoming units (this round): Full opacity
   - Next round units: Below separator

## Example Layout

```
┌─────────────────┐
│   Turn Order    │
├─────────────────┤
│ ► Knight [YOU]  │  ← Active (bright yellow)
│   Goblin        │  ← Next this round
│   Archer [YOU]  │  ← Next this round
├═════════════════┤  ← Round Separator
│   Knight [YOU]  │  ← Next round (faded)
│   Goblin        │  ← Next round (faded)
└─────────────────┘
```

## Testing

1. Enter Play Mode
2. Check that units appear in speed order
3. Verify active unit is at top with highlighting
4. Confirm round separator appears at the correct position
5. Watch the list update as turns progress
6. Verify the separator moves correctly each round

## Troubleshooting

- **Entries not appearing**: Check that prefabs are assigned in InitiativeOrderUI
- **Order wrong**: Verify unit speed values in UnitData_SO
- **Separator not showing**: Ensure Round Separator Prefab is assigned
- **Layout broken**: Check Vertical Layout Group and Content Size Fitter settings
