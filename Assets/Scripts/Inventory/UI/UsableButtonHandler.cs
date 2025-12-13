using UnityEngine;
using System.Collections;

public class UsableButtonHandler : MonoBehaviour
{
    [Header("Usable Button Handler Settings")]
    public bool buttonPressed = false;

    [Header("References")]
    public InventoryUI inventoryUI;
    public HealingUsable healingUsable;


    private void Awake()
    {

    }

    public void OnButtonPress()
    {
        buttonPressed = true;

        // Find the selected slot
        ItemSlot selectedSlot = null;
        foreach (var slot in inventoryUI.itemSlots)
        {
            if (slot != null && slot.thisItemSelected && slot.isFull)
            {
                selectedSlot = slot;
                break;
            }
        }

        // Example: Check for healing item by ID
        if (selectedSlot.ItemId == "HP_Potion_1")
        {
            healingUsable.UsableHealPlayer();
        }
        // Remove the item from the inventory
        // Find the actual Item object in the inventory by ID and name
        Item itemToRemove = null;
        foreach (var item in inventoryUI.inventory.Items)
        {
            if (item != null && item.Id == selectedSlot.ItemId && item.ItemName == selectedSlot.itemName)
            {
                itemToRemove = item;
                break;
            }
        }

        if (itemToRemove != null)
        {
            inventoryUI.inventory.RemoveItem(itemToRemove);
        }
        else
        {
            Debug.LogWarning("Item to remove not found in inventory.");
        }

        // Refresh the UI
        inventoryUI.RefreshSlots();
        buttonPressed = false;
    }
}
