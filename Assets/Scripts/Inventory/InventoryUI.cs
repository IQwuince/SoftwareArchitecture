using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject inventoryUIObject;
    public TextMeshProUGUI descriptionText;
    public ItemSlot[] itemSlots;
    public Button usableButton;

    private void Awake()
    {
        // link shared description text for slots
        ItemSlot.SharedDescriptionText = descriptionText;

        if (usableButton != null)
            usableButton.gameObject.SetActive(false);

        // subscribe to slot click broadcasts
        ItemSlot.OnAnySlotClicked += OnSlotSelected;
    }

    private void OnDestroy()
    {
        ItemSlot.OnAnySlotClicked -= OnSlotSelected;
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

    public void PrintItemCountById(string soId)
    {
        if (inventory == null || string.IsNullOrEmpty(soId))
        {
            Debug.LogWarning("Inventory or ID is null/empty.");
            return;
        }

        int count = 0;
        string itemName = "";

        foreach (var item in inventory.Items)
        {
            if (item != null && item.Id == soId)
            {
                count++;
                itemName = item.ItemName;
            }
        }

        if (count > 0)
        {
            Debug.Log($"Item: {itemName} (ID: {soId}) - Count: {count}");
        }
        else
        {
            Debug.Log($"No items found with ID: {soId}");
        }
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
                slot.AddItem(item.ItemName, 1, item.itemIcon, item.ItemDescription, item.Id);
            }
            else
            {
                slot.Clear();
            }
        }

        if (descriptionText != null)
            descriptionText.text = "";

        if (usableButton != null)
            usableButton.gameObject.SetActive(false);

        DeselectAllSlots();
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
        if (Keyboard.current != null && Keyboard.current.yKey.wasPressedThisFrame)
        {
            PrintItemCountById("HP_Potion_1");
        }
    }

    private void DeselectAllSlots()
    {
        if (itemSlots == null) return;
        foreach (var slot in itemSlots)
        {
            if (slot != null)
                slot.SetSelected(false);
        }
    }

    private void OnSlotSelected(ItemSlot slot)
    {
        // make selection exclusive
        DeselectAllSlots();

        if (slot != null)
            slot.SetSelected(true);

        // show usable button only for hp potion id (adjust to your usable detection later)
        if (usableButton != null)
        {
            if (slot != null && slot.isFull && slot.ItemId == "HP_Potion_1")
                usableButton.gameObject.SetActive(true);
            else
                usableButton.gameObject.SetActive(false);
        }

    }
}