# Collect Quest Integration with Inventory System

## Overview
The collect quest system has been updated to properly integrate with the inventory system using `ItemData` ScriptableObjects instead of GameObject references.

## What Changed

### 1. CollectQuestSO (Collect Quest ScriptableObject)
- **New Field**: `itemToCollect` (ItemData) - Reference to the ItemData ScriptableObject that needs to be collected
- **Custom Validation**: Validates that `itemToCollect` is assigned (instead of the old `target` GameObject)
- The old `target` field is still in the base class but is not used for collect quests

### 2. QuestSO (Base Quest ScriptableObject)
- **Updated Validation**: The `target` field is now only required for Kill quests
- Collect quests don't need to set the `target` field anymore

### 3. Inventory
- **New Method**: `GetItemCount(ItemData itemData)` - Returns how many items of a specific ItemData type are in the inventory
- Uses the ItemData's `id` field to match items in the inventory

### 4. QuestManager
- **New Field**: `playerInventory` (Inventory) - Reference to the player's inventory (auto-detected if not set)
- **Updated Logic**: `HandleItemPickedUp` now checks the inventory count using ItemData instead of GameObject matching
- **Helper Method**: `FindPlayerInventory()` - Automatically finds the player inventory via SingletonPlayerInventoryController or fallback to any Inventory component
- **Deprecated Method**: `UpdateCollectProgress(GameObject)` - No longer functional for collect quests, kept for backward compatibility

## How to Use

### Setting Up a Collect Quest

1. **Create a Collect Quest**:
   - Right-click in Project → Create → Quests → Collect Items Quest

2. **Configure the Quest**:
   - Set `id`, `title`, and `description` as before
   - Set `targetCount` to the number of items needed
   - **NEW**: Assign the `Item To Collect` field to an ItemData ScriptableObject (from your inventory system)
   - Do NOT set the `target` field (it's not used for collect quests anymore)

3. **Assign to QuestManager** (optional):
   - In the scene, select the QuestManager
   - Optionally assign your Inventory reference to the `Player Inventory` field
   - If not assigned, it will auto-detect the inventory at runtime

### Example Workflow

```
1. Create/Find an ItemData ScriptableObject (e.g., "RuneData")
2. Create a CollectQuestSO
3. Assign the ItemData to the "Item To Collect" field
4. Set targetCount to 5 (for example)
5. When the player picks up 5 items with matching ItemData, the quest completes
```

## How It Works

1. **Quest Activation**: When a collect quest is activated, it validates that `itemToCollect` is set
2. **Item Collection**: When the player picks up an item (via the Inventory system), the `Inventory.OnItemPickedUp` event fires
3. **Progress Tracking**: The QuestManager receives the event and:
   - Gets the active collect quest
   - Checks if it has a valid `itemToCollect` ItemData
   - Queries the player's inventory using `GetItemCount(itemToCollect)`
   - Updates the quest progress to match the current inventory count
4. **Quest Completion**: When the inventory count reaches `targetCount`, the quest auto-completes

## Benefits

✅ **Proper Integration**: Uses the existing inventory system's ItemData ScriptableObjects
✅ **Easy to Configure**: Just assign the ItemData you want players to collect
✅ **Accurate Tracking**: Progress is based on actual inventory count
✅ **Flexible**: Can track any item that exists in your inventory system
✅ **Organized**: Keeps everything using the same ScriptableObject architecture

## Migration Notes

### For Existing Collect Quests
If you have existing CollectQuestSO assets:
1. Open each CollectQuestSO in the inspector
2. Assign the appropriate ItemData to the new `Item To Collect` field
3. You can leave the old `target` field as is (it will be ignored)

### For New Collect Quests
Simply assign the `Item To Collect` field and ignore the `target` field.

## Technical Details

### Inventory Count Matching
The system matches items using the ItemData's `id` field:
```csharp
public int GetItemCount(ItemData itemData)
{
    if (itemData == null) return 0;
    
    int count = 0;
    foreach (Item item in items)
    {
        if (item.Id == itemData.id)
        {
            count++;
        }
    }
    return count;
}
```

### Auto-Detection of Inventory
If the `playerInventory` field is not set in the QuestManager, it will automatically try to find it:
1. First via `SingletonPlayerInventoryController.Instance.inventory`
2. Fallback to `FindObjectOfType<Inventory>()`

This ensures the system works even if the reference isn't manually set.

## Testing

### Manual Testing
1. Create a collect quest with an ItemData reference
2. Activate the quest (press P in the demo scene)
3. Pick up items of that ItemData type
4. Verify the quest progress updates in the UI
5. Verify the quest completes when reaching the target count

### Programmatic Testing
```csharp
// Activate quest
questManager.ActivateQuest(myCollectQuest);

// Simulate item pickup by adding to inventory
// (The quest will auto-update via the Inventory.OnItemPickedUp event)
playerInventory.AddItem(itemData.CreateItem());
```

## Troubleshooting

### Quest Progress Not Updating
- **Check**: Is the `Item To Collect` field assigned in the CollectQuestSO?
- **Check**: Does the inventory contain items with matching ItemData.id?
- **Check**: Is the Inventory component firing the `OnItemPickedUp` event?

### "Player inventory not found" Warning
- **Solution**: Assign the `Player Inventory` field in the QuestManager inspector
- **Alternative**: Ensure there's a SingletonPlayerInventoryController or Inventory component in the scene

### Validation Error
- **Error**: "Item to collect is required for collect quests"
- **Solution**: Assign an ItemData ScriptableObject to the `Item To Collect` field
