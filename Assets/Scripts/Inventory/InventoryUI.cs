using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject inventoryUIObject;
    public TextMeshProUGUI descriptionText;
    public ItemSlot[] itemSlots;

    private void Awake()
    {
        ItemSlot.SharedDescriptionText = descriptionText;
    }

    private void OnEnable()
    {
        Inventory.OnItemPickedUp += OnInventoryChanged;
    }

    private void OnDisable()
    {
        Inventory.OnItemPickedUp -= OnInventoryChanged;
    }

    private void OnInventoryChanged(string itemName)
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        if (inventory == null) return;
        if (itemSlots == null || itemSlots.Length == 0)
            itemSlots = GetComponentsInChildren<ItemSlot>(true);
        if (itemSlots == null || itemSlots.Length == 0) return;

        Item[] items = inventory.Items;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];
            if (slot == null) continue;
            if (i < items.Length && items[i] != null)
            {
                Item item = items[i];
                slot.AddItem(item.ItemName, 1, item.itemIcon, item.ItemDescription);
            }
            else
            {
                slot.Clear();
            }
        }
        if (descriptionText != null)
            descriptionText.text = "";
    }

    public void InventoryItems()
    {
        RefreshSlots();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            InventoryItems();
        }
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (inventoryUIObject != null)
            {
                inventoryUIObject.SetActive(!inventoryUIObject.activeSelf);
                InventoryItems();
            }
        }
    }
}
