# Quest System Usage Guide

## Overview

This quest system provides a pluggable, ScriptableObject-based framework for creating and managing quests in your Unity 2D dungeon crawler. It supports two quest types: **Kill Enemies** and **Collect Items**.

## Quick Start

### 1. Creating a Quest

1. Right-click in the Project window
2. Navigate to `Create > Quests`
3. Choose either:
   - **Kill Enemies Quest** - for enemy-killing quests
   - **Collect Items Quest** - for item-collection quests

### 2. Configuring a Quest

Fill in the quest properties in the Inspector:

#### For Kill Quests:
- **ID**: Unique identifier for the quest
- **Title**: Display name shown in UI
- **Description**: Quest description for players
- **Quest Type**: Automatically set to Kill
- **Target**: Reference to the enemy prefab to track
- **Target Count**: Number of enemies needed to complete
- **Reward**: Configure XP, item ID, and currency rewards
- **Icon**: Optional sprite for UI display

#### For Collect Quests:
- **ID**: Unique identifier for the quest
- **Title**: Display name shown in UI
- **Description**: Quest description for players
- **Quest Type**: Automatically set to Collect
- **Item To Collect**: Reference to the ItemData ScriptableObject to collect (from inventory system)
- **Target Count**: Number of items needed to complete
- **Reward**: Configure XP, item ID, and currency rewards
- **Icon**: Optional sprite for UI display

**Note**: Collect quests now use the inventory system's ItemData ScriptableObjects instead of GameObject references. This allows proper integration with the inventory system.

The editor will show validation warnings if required fields are missing or invalid.

### 3. Activating Quests

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

### 4. Quest Progress Rules

- **One Active Quest Per Type**: Only one Kill quest and one Collect quest can be active at a time
- **Auto-Completion**: Quests complete automatically when target count is reached
- **Event-Driven**: Progress updates happen via game events (enemy kills, item pickups)
- **Inventory-Based Collection**: Collect quests track progress by checking the player's inventory count for specific ItemData

## Integration API

### For Enemy Death

The quest system automatically listens to the existing `EnemyHealth.OnEnemyKilled` event. Alternatively, call directly:

```csharp
questManager.UpdateKillProgress(enemyPrefab);
```

### For Item Collection

The collect quest system now automatically tracks items via the inventory system. When items are added to the inventory:

1. The `Inventory.OnItemPickedUp` event fires
2. The quest system checks if there's an active collect quest
3. If the quest's `itemToCollect` matches items in the inventory, progress updates automatically

**No manual calls needed** - the quest system monitors the inventory count automatically!

#### Legacy Method (Deprecated)
The old `UpdateCollectProgress(GameObject)` method is deprecated:

```csharp
// This method is deprecated and no longer functional for collect quests
questManager.UpdateCollectProgress(itemPrefab);
```

### Setting Up Inventory Reference

In the QuestManager component:
- Optionally assign the `Player Inventory` field in the inspector
- If not assigned, it will auto-detect via SingletonPlayerInventoryController or FindObjectOfType
```

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
        // Award rewards
        var reward = quest.questData.reward;
        // player.AddXP(reward.xp);
        // player.AddCurrency(reward.currency);
        // inventory.AddItem(reward.itemId);
    }
}
```

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
