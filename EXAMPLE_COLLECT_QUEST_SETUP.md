# Example: Setting Up a Collect Quest with Inventory Integration

This example demonstrates how to set up a collect quest that properly integrates with the inventory system.

## Scenario
We want to create a quest where the player needs to collect 5 "Test 1" items.

## Step-by-Step Guide

### Step 1: Ensure You Have an ItemData Asset

We already have `Test1.asset` in the project:
- **Location**: `Assets/Scriptable Objects/Test1.asset`
- **ID**: `Test_item1`
- **Name**: `Test 1`

This ItemData represents the item we want the player to collect.

### Step 2: Create or Update the Collect Quest

1. Open the existing `New Collect Quest.asset` or create a new one:
   - Right-click in Project → Create → Quests → Collect Items Quest

2. Configure the quest properties:
   ```
   ID: CollectTest1Items
   Title: Collect Test Items
   Description: Collect 5 Test 1 items from the dungeon
   Quest Type: Collect (auto-set)
   Item To Collect: [Assign Test1.asset here]
   Target Count: 5
   Reward:
     - XP: 200
     - Item ID: 0
     - Currency: 50
   Icon: [Optional]
   ```

3. **Important**: Do NOT set the "Target" field (the GameObject one) - it's not used for collect quests anymore.

### Step 3: Set Up the QuestManager

1. In your scene, locate or create the QuestManager GameObject
2. Assign the collect quest to the `Sample Collect Quest` field (for testing with P key)
3. Optionally assign the player's Inventory to the `Player Inventory` field
   - If not assigned, it will auto-detect at runtime

### Step 4: How It Works

#### When the Quest is Activated

```csharp
// Activate the quest (or press P key in play mode)
questManager.ActivateQuest(collectQuest);
```

The quest validates that:
- ✅ ID and title are set
- ✅ Target count > 0
- ✅ Item To Collect is assigned (Test1.asset)

#### When Items Are Collected

When the player picks up an item through the inventory system:

```csharp
// In your item pickup code
ItemContainer itemContainer = GetComponent<ItemContainer>();
Item item = itemContainer.GiveItem(); // This triggers the inventory system
```

The flow is:
1. `ItemContainer.GiveItem()` fires the `ItemContainer.onGetItem` event
2. `SingletonPlayerInventoryController` receives the event and calls `inventory.AddItem(item)`
3. `Inventory.AddItem()` fires the `Inventory.OnItemPickedUp` event with the item name
4. `QuestManager.HandleItemPickedUp()` receives the event
5. QuestManager checks if there's an active collect quest
6. QuestManager checks if the quest's `itemToCollect` matches items in the inventory using `inventory.GetItemCount(collectQuest.itemToCollect)`
7. If the count increased, the quest progress is updated

#### Progress Tracking

The quest progress is based on the **actual inventory count**:
- Player has 0 Test1 items → Quest shows 0/5
- Player picks up 1 Test1 item → Quest shows 1/5
- Player picks up 2 more Test1 items → Quest shows 3/5
- Player picks up 2 more Test1 items → Quest shows 5/5 and completes!

#### Quest Completion

When the inventory count reaches the target count (5 in this example):
- The quest automatically completes
- `QuestManager.OnQuestCompleted` event fires
- The quest is removed from active quests
- Rewards can be awarded in a reward handler

## Code Example: Complete Integration

### Creating a Reward Handler

```csharp
using UnityEngine;
using IQwuince.Quests;

public class QuestRewardHandler : MonoBehaviour
{
    public QuestManager questManager;

    private void OnEnable()
    {
        if (questManager != null)
        {
            questManager.OnQuestCompleted += HandleQuestCompleted;
        }
    }

    private void OnDisable()
    {
        if (questManager != null)
        {
            questManager.OnQuestCompleted -= HandleQuestCompleted;
        }
    }

    private void HandleQuestCompleted(Quest quest)
    {
        Debug.Log($"Quest '{quest.questData.title}' completed!");
        
        var reward = quest.questData.reward;
        
        // Award XP
        if (reward.xp > 0)
        {
            // playerStats.AddXP(reward.xp);
            Debug.Log($"Awarded {reward.xp} XP");
        }
        
        // Award currency
        if (reward.currency > 0)
        {
            // playerInventory.AddCurrency(reward.currency);
            Debug.Log($"Awarded {reward.currency} currency");
        }
        
        // Award item
        if (reward.itemId > 0)
        {
            // ItemData itemData = GetItemDataById(reward.itemId);
            // playerInventory.AddItem(itemData.CreateItem());
            Debug.Log($"Awarded item with ID {reward.itemId}");
        }
    }
}
```

### Testing in Play Mode

1. Enter Play Mode in Unity
2. Press **P** to activate the sample collect quest
3. Pick up Test1 items in the game
4. Watch the quest UI update automatically
5. When you collect 5 items, the quest completes

## Debugging Tips

### Enable Debug Logs

The QuestManager already has debug logs:
```csharp
Debug.Log($"Quest progress: {collectQuest.questData.title} - {collectQuest.GetProgressString()}");
```

You'll see messages like:
```
Quest activated: Collect Test Items
Quest progress: Collect Test Items - 1/5
Quest progress: Collect Test Items - 2/5
...
Quest completed: Collect Test Items! Rewards: XP=200, ItemID=0, Currency=50
```

### Common Issues

**Progress not updating?**
- Check that the `Item To Collect` field is assigned in the collect quest
- Verify that items being picked up have the same ItemData.id as the quest requirement
- Ensure the Inventory component is firing the `OnItemPickedUp` event

**"Player inventory not found" warning?**
- Assign the Inventory to the QuestManager's `Player Inventory` field
- Or ensure there's a SingletonPlayerInventoryController in the scene

**Quest won't activate?**
- Check the Console for validation errors
- Make sure all required fields are filled in the quest asset

## Comparing Old vs New Approach

### OLD (GameObject-based):
```
CollectQuestSO:
  Target: [GameObject reference to item prefab]
  
Problem: 
  - Had to match by GameObject name
  - Didn't integrate with inventory system
  - Couldn't track actual inventory contents
```

### NEW (ItemData-based):
```
CollectQuestSO:
  Item To Collect: [ItemData ScriptableObject]
  
Benefits:
  ✅ Uses same ItemData as inventory system
  ✅ Tracks actual inventory contents
  ✅ Easy to configure
  ✅ Consistent with game architecture
```

## Summary

The new inventory integration makes collect quests:
1. **More accurate** - tracks actual inventory contents
2. **Easier to configure** - just assign the ItemData you want
3. **Better integrated** - uses the existing inventory system
4. **More maintainable** - consistent ScriptableObject architecture

No manual tracking needed - the system automatically monitors the inventory!
