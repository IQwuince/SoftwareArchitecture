# Quest System Usage Guide

## Overview

This quest system provides a pluggable, ScriptableObject-based framework for creating and managing quests in your Unity 2D dungeon crawler. It supports two quest types: **Kill Enemies** and **Collect Items**.

The system integrates with the existing **Inventory** system for item collection tracking and the **LevelSystem** for XP rewards.

## Quick Start

### 1. Creating a Quest

1. Right-click in the Project window
2. Navigate to `Create > Quests`
3. Choose either:
   - **Kill Enemies Quest** - for enemy-killing quests
   - **Collect Items Quest** - for item-collection quests

### 2. Configuring a Quest

Fill in the quest properties in the Inspector:

**For Kill Quests:**
- **ID**: Unique identifier for the quest
- **Title**: Display name shown in UI
- **Description**: Quest description for players
- **Quest Type**: Automatically set to Kill
- **Target Enemy**: Reference to the enemy prefab to kill
- **Target Count**: Number of enemies needed to complete
- **Reward XP**: Experience points awarded on completion
- **Reward Items**: Array of ItemData ScriptableObjects to award (optional)
- **Icon**: Optional sprite for UI display

**For Collect Quests:**
- **ID**: Unique identifier for the quest
- **Title**: Display name shown in UI
- **Description**: Quest description for players
- **Quest Type**: Automatically set to Collect
- **Target Item**: ItemData ScriptableObject to collect from inventory
- **Target Count**: Number of items needed to complete
- **Reward XP**: Experience points awarded on completion
- **Reward Items**: Array of ItemData ScriptableObjects to award (optional)
- **Icon**: Optional sprite for UI display

The editor will show validation warnings if required fields are missing or invalid.

### 3. Setting Up QuestManager

The QuestManager needs references to work properly:

1. Add QuestManager to your scene (or use the QuestManager prefab)
2. Assign **Player Inventory** reference (Inventory component)
3. Assign **Player Level System** reference (LevelSystem component)
4. Optionally assign sample quests for testing

### 4. Activating Quests

#### Option A: Sample Scene Testing (O/P Keys)
In the QuestDemo scene:
- Press **O** to activate the sample Kill quest
- Press **P** to activate the sample Collect quest

#### Option B: Programmatic Activation
```csharp
using IQwuince.Quests;

public class YourScript : MonoBehaviour
{
    public QuestManager questManager;
    public QuestSO questToActivate;

    void ActivateMyQuest()
    {
        bool success = questManager.ActivateQuest(questToActivate);
        if (!success)
        {
            Debug.Log("Quest activation failed (already active or invalid)");
        }
    }
}
```

### 5. Quest Progress Rules

- **One Active Quest Per Type**: Only one Kill quest and one Collect quest can be active at a time
- **Auto-Completion**: Quests complete automatically when target count is reached
- **Event-Driven**: Progress updates happen via game events (enemy kills, item pickups from inventory)

## Integration API

### For Enemy Death

The quest system automatically listens to the existing `EnemyHealth.OnEnemyKilled` event. Alternatively, call directly:

```csharp
questManager.UpdateKillProgress(enemyPrefab);
```

### For Item Collection

The quest system automatically tracks items in the inventory when `Inventory.OnItemPickedUp` is fired. The system checks the inventory to count items matching the quest's target ItemData.

**No manual calls needed** - the system uses the inventory to track progress automatically!

### Listening to Quest Events

```csharp
using IQwuince.Quests;

public class RewardHandler : MonoBehaviour
{
    public QuestManager questManager;

    void OnEnable()
    {
        questManager.OnQuestActivated += HandleQuestActivated;
        questManager.OnQuestProgressChanged += HandleQuestProgress;
        questManager.OnQuestCompleted += HandleQuestCompleted;
    }

    void OnDisable()
    {
        questManager.OnQuestActivated -= HandleQuestActivated;
        questManager.OnQuestProgressChanged -= HandleQuestProgress;
        questManager.OnQuestCompleted -= HandleQuestCompleted;
    }

    void HandleQuestActivated(Quest quest)
    {
        Debug.Log($"Quest started: {quest.questData.title}");
    }

    void HandleQuestProgress(Quest quest)
    {
        Debug.Log($"Progress: {quest.GetProgressString()}");
    }

    void HandleQuestCompleted(Quest quest)
    {
        // Rewards are automatically given by QuestManager
        // XP is added to LevelSystem
        // Items are added to Inventory
        Debug.Log($"Quest completed: {quest.questData.title}");
    }
}
```

## Reward System

The quest system now supports flexible rewards:

### XP Rewards
- Automatically added to the player's LevelSystem when quest completes
- Configure in the "Reward XP" field

### Item Rewards
- Award one or more ItemData ScriptableObjects
- Items are automatically added to the player's inventory on quest completion
- Configure in the "Reward Items" array
- Easy to extend: just add more items to the array!

### Example Reward Configuration
```
Reward XP: 100
Reward Items: 
  - Element 0: Sword ItemData
  - Element 1: Shield ItemData
  - Element 2: Potion ItemData
```

All rewards are automatically given when the quest completes - no manual code needed!

## UI Display

The quest UI automatically updates to show all active quests in this format:

```
[Kill] Slay Goblins: 3/10
[Collect] Gather Runes: 2/5
```

When no quests are active, it shows instructions for the sample scene.

## Sample Content

The system includes two sample quests:

1. **Kill Goblins** (activated with O key)
   - Kill 10 enemy creatures
   
2. **Gather Runes** (activated with P key)
   - Collect 5 rune items

Test these in the **QuestDemo.unity** scene.

## Architecture Notes

### Folder Structure
```
Assets/
├── Scripts/
│   └── Quests/
│       ├── Editor/
│       │   └── QuestSOEditor.cs
│       ├── QuestSO.cs
│       ├── KillQuestSO.cs
│       ├── CollectQuestSO.cs
│       ├── Quest.cs
│       ├── QuestManager.cs
│       ├── QuestUIManager.cs
│       ├── QuestTypes.cs
│       └── QuestReward.cs
└── ScriptableObjects/
    └── Quests/
        ├── KillGoblins.asset
        └── GatherRunes.asset
```

### Key Components

- **QuestSO**: Base ScriptableObject for quest data
- **KillQuestSO/CollectQuestSO**: Concrete quest types
- **Quest**: Runtime instance representing active quest state
- **QuestManager**: Singleton-style manager handling activation and progress
- **QuestUIManager**: TMP-based UI display component

### Performance

- Event-driven updates minimize performance overhead
- Dictionary-based lookups for O(1) quest access
- No per-frame allocations during normal operation

## Troubleshooting

### Quest won't activate
- Check that no quest of the same type is already active
- Verify quest data validation passes (check Inspector)
- Ensure target prefab is assigned

### Progress not updating
- Verify enemy prefab reference matches exactly
- Check that events are being fired (`EnemyHealth.OnEnemyKilled`, `Inventory.OnItemPickedUp`)
- Ensure QuestManager is in the scene and enabled

### UI not displaying
- Verify QuestUIManager has references to QuestManager and TMP Text
- Check that TMP text component is active and visible
- Ensure Canvas is set up correctly

## Extending the System

To add new quest types:

1. Create a new enum value in `QuestType`
2. Create a new `QuestSO` subclass
3. Add handler logic in `QuestManager`
4. Update UI formatting in `QuestUIManager` if needed

The system is designed to be modular and extensible while keeping the core logic simple.
