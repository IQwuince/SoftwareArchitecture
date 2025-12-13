using UnityEngine;

public class HealingUsable : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        inventoryUI = UnityEngine.Object.FindFirstObjectByType<InventoryUI>();
    }

    public void UsableHealPlayer()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth reference not found.");
            return;
        }

        if (inventoryUI == null || inventoryUI.itemSlots == null || inventoryUI.inventory == null)
        {
            Debug.LogWarning("InventoryUI or inventory reference not found.");
            return;
        }

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

        if (selectedSlot == null)
        {
            Debug.LogWarning("No item slot is selected.");
            return;
        }

        // Find the corresponding Item in the inventory
        Item selectedItem = null;
        foreach (var item in inventoryUI.inventory.Items)
        {
            if (item != null && item.Id == selectedSlot.ItemId && item.ItemName == selectedSlot.itemName)
            {
                selectedItem = item;
                break;
            }
        }

        if (selectedItem == null)
        {
            Debug.LogWarning("Selected item not found in inventory.");
            return;
        }

        // Use the helper on Item to get heal amount
        if (!selectedItem.IsUsable)
        {
            Debug.LogWarning("Selected item is not usable.");
            return;
        }

        int healAmount = selectedItem.HealAmount;

        if (healAmount > 0)
        {
            playerHealth.Heal(healAmount);
        }
        else
        {
            Debug.LogWarning("Heal amount is zero or missing on the Usables SO.");
        }
    }
}