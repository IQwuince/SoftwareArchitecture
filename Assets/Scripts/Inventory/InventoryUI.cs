using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Inventory inventory;
    public GameObject inventoryUIObject;

    private void GetNames()
    {
        foreach (Item item in inventory.Items)
        {
            itemNameText.text += item.ItemName + "\n";
        }
    }

    private void ResetNames()
    {
        itemNameText.text = "";
    }

    public void InventoryItems()
    {
        ResetNames();
        GetNames();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            Debug.Log("GetNames");
            InventoryItems();
        }

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            inventoryUIObject.SetActive(!inventoryUIObject.activeSelf);
            InventoryItems();
        }
    }
}
