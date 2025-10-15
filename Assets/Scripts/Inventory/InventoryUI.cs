using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public Inventory inventory;

   public void GetNames()
    {
        foreach (Item item in inventory.Items)
        {
            itemNameText.text += item.ItemName + "\n";
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            Debug.Log("GetNames");
            GetNames();
        }
    }
}
